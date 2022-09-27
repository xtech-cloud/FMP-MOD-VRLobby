

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.VRLobby.LIB.Proto;
using XTC.FMP.MOD.VRLobby.LIB.MVCS;
using System.Linq;
using Newtonsoft.Json;

namespace XTC.FMP.MOD.VRLobby.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        private MyContent activeContent_;
        private MyCatalog.Section activeSection_;
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
                    activeSection_ = section;
                    var content = activeSection_.contentS.First();
                    loadContentFromAsset(content, (_bytes) =>
                    {
                        try
                        {
                            activeContent_ = JsonConvert.DeserializeObject<MyContent>(System.Text.Encoding.UTF8.GetString(_bytes));
                        }
                        catch (System.Exception ex)
                        {
                            logger_.Exception(ex);
                        }
                        openResource();
                    });
                });
                enterButtons.Add(tEntry);
            }
        }

        private void openResource()
        {
            if (null == activeContent_)
            {
                logger_.Error("None content is active"); ;
                return;
            }

            logger_.Debug("active content is {0}/{1}", activeContent_.bundle, activeContent_.alias);
            string downstream;
            activeSection_.kvS.TryGetValue("downstream", out downstream);
            if (string.IsNullOrWhiteSpace(downstream))
            {
                logger_.Error("downstream is null or empty"); ;
                return;
            }
            logger_.Debug("downstream is {0}", downstream);

            Dictionary<string, string> messageReplaces = new Dictionary<string, string>();
            messageReplaces["{{downstream}}"] = downstream;

            string resource_uri;
            activeContent_.kvS.TryGetValue(downstream, out resource_uri);
            if (string.IsNullOrWhiteSpace(resource_uri))
            {
                logger_.Error("resource_uri is null or empty"); ;
                return;
            }
            logger_.Debug("resource_uri is {0}", resource_uri);

            Dictionary<string, string> parameterReplaces = new Dictionary<string, string>();
            messageReplaces["{{resource_uri}}"] = resource_uri;

            foreach (var subject in style_.bannerMenu.bannerEntry.onSubjects)
            {
                publishSubject(subject, messageReplaces, parameterReplaces);
            }
        }

        private void publishSubject(MyConfig.Subject _subject, Dictionary<string, string> _messageReplaces, Dictionary<string, string> _parameterReplaces)
        {
            var dummyModel = (entry_ as MyEntry).getDummyModel();
            var data = new Dictionary<string, object>();
            foreach (var parameter in _subject.parameters)
            {
                if (parameter.type.Equals("string"))
                {
                    string strValue = parameter.value;
                    foreach (var pair in _parameterReplaces)
                    {
                        strValue = strValue.Replace(pair.Key, pair.Value);
                    }
                    data[parameter.key] = strValue;
                }
                else if (parameter.type.Equals("int"))
                    data[parameter.key] = int.Parse(parameter.value);
                else if (parameter.type.Equals("float"))
                    data[parameter.key] = float.Parse(parameter.value);
                else if (parameter.type.Equals("bool"))
                    data[parameter.key] = bool.Parse(parameter.value);
            }
            string message = _subject.message;
            foreach (var pair in _messageReplaces)
            {
                message = message.Replace(pair.Key, pair.Value);
            }
            dummyModel.Publish(message, data);
        }
    }
}
