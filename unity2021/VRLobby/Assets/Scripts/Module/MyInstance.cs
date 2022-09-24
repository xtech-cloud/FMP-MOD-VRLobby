

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.VRLobby.LIB.Proto;
using XTC.FMP.MOD.VRLobby.LIB.MVCS;

namespace XTC.FMP.MOD.VRLobby.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        private List<Transform> enterButtons = new List<Transform>();
        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        public void HandleCreated()
        {
            // 使用序列器等待主题资源加载完成后再应用大纲
            var sequence = applyStyle();
            sequence.OnFinish = () =>
            {
                applyCatalog();
            };
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        public void HandleOpened(string _source, string _uri)
        {
            rootUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            rootUI.gameObject.SetActive(false);
        }

        private void publishSubject(MyConfig.Subject _subject)
        {
            var dummyModel = (entry_ as MyEntry).getDummyModel();
            var data = new Dictionary<string, object>();
            foreach (var parameter in _subject.parameters)
            {
                if (parameter.type.Equals("string"))
                    data[parameter.key] = parameter.value;
                else if (parameter.type.Equals("int"))
                    data[parameter.key] = int.Parse(parameter.value);
                else if (parameter.type.Equals("float"))
                    data[parameter.key] = float.Parse(parameter.value);
                else if (parameter.type.Equals("bool"))
                    data[parameter.key] = bool.Parse(parameter.value);
            }
            dummyModel.Publish(_subject.message, data);
        }

        private CounterSequence applyStyle()
        {
            CounterSequence sequence = new CounterSequence(0);
            rootUI.transform.Find("BannerMenu").gameObject.SetActive(style_.active.Equals("BannerMenu"));
            if (style_.active.Equals("BannerMenu"))
            {
                // 设置距离
                var position = rootUI.transform.localPosition;
                position.z = style_.bannerMenu.distance;
                rootUI.transform.localPosition = position;

                // 加载标题图
                var tTitle = rootUI.transform.Find("BannerMenu/imgTitle");
                alignByAncor(tTitle, style_.bannerMenu.title.anchor);
                sequence.Dial();
                loadSpriteFromTheme(style_.bannerMenu.title.image, (_sprite) =>
                {
                    tTitle.GetComponent<Image>().sprite = _sprite;
                    sequence.Tick();
                });

                // 加载条幅图
                var tBanner = rootUI.transform.Find("BannerMenu/banner");
                alignByAncor(tBanner, style_.bannerMenu.bannerEntry.banner.anchor);
                sequence.Dial();
                loadSpriteFromTheme(style_.bannerMenu.bannerEntry.banner.image, (_sprite) =>
                {
                    tBanner.GetComponent<Image>().sprite = _sprite;
                    sequence.Tick();
                });

                // 加载条幅凝视图
                var tBannerGaze = rootUI.transform.Find("BannerMenu/banner/__gaze__");
                sequence.Dial();
                loadSpriteFromTheme(style_.bannerMenu.bannerEntry.banner.gazeImage, (_sprite) =>
                {
                    tBannerGaze.GetComponent<Image>().sprite = _sprite;
                    sequence.Tick();
                });

                // 加载入口图
                var tEntry = rootUI.transform.Find("BannerMenu/banner/entry");
                alignByAncor(tEntry, style_.bannerMenu.bannerEntry.entry.anchor);
                sequence.Dial();
                loadSpriteFromTheme(style_.bannerMenu.bannerEntry.entry.image, (_sprite) =>
                {
                    tEntry.GetComponent<Image>().sprite = _sprite;
                    sequence.Tick();
                });

                // 隐藏入口按钮
                tEntry.gameObject.SetActive(false);

                // 加载入口凝视图
                var tEntryGaze = rootUI.transform.Find("BannerMenu/banner/entry/__gaze__");
                sequence.Dial();
                loadSpriteFromTheme(style_.bannerMenu.bannerEntry.entry.gazeImage, (_sprite) =>
                {
                    tEntryGaze.GetComponent<Image>().sprite = _sprite;
                    sequence.Tick();
                });

                tBanner.gameObject.SetActive(false);
            }
            return sequence;
        }

        private void applyCatalog()
        {
            enterButtons.Clear();
            var tBanner = rootUI.transform.Find("BannerMenu/banner");
            var anchor = style_.bannerMenu.bannerEntry.banner.anchor;
            foreach (var section in catalog_.sectionS)
            {
                GameObject clone = GameObject.Instantiate(tBanner.gameObject, tBanner.parent);

                string strMarginH;
                if (!section.kvS.TryGetValue("marginH", out strMarginH))
                {
                    strMarginH = "0";
                }
                anchor.marginH = strMarginH;
                alignByAncor(clone.transform, anchor);

                string strImage;
                if (section.kvS.TryGetValue("image", out strImage))
                {
                    loadSpriteFromTheme(strImage, (_sprite) =>
                    {
                        clone.GetComponent<Image>().sprite = _sprite;
                    });
                }

                clone.gameObject.SetActive(true);

                var tEntry = clone.transform.Find("entry");
                clone.GetComponent<Button>().onClick.AddListener(() =>
                {
                    foreach (var button in enterButtons)
                    {
                        button.gameObject.SetActive(button == tEntry);
                    }
                });

                tEntry.GetComponent<Button>().onClick.AddListener(() =>
                {
                    foreach (var subject in style_.bannerMenu.bannerEntry.subjects)
                    {
                        publishSubject(subject);
                    }
                });
                enterButtons.Add(tEntry);
            }
        }
    }
}
