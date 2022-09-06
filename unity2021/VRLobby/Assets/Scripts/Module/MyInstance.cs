

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
        public MyInstance(string _uid, string _style, MyConfig _config, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        public void HandleCreated()
        {
            enterButtons.Clear();
            rootUI.transform.Find("BannerMenu").gameObject.SetActive(style_.active.Equals("BannerMenu"));
            if (style_.active.Equals("BannerMenu"))
            {
                // 设置距离
                var position = rootUI.transform.localPosition;
                position.z = style_.bannerMenu.distance;
                rootUI.transform.localPosition = position;

                var tTitle = rootUI.transform.Find("BannerMenu/imgTitle");
                alignByAncor(tTitle, style_.bannerMenu.title.anchor);
                loadSpriteFromTheme(style_.bannerMenu.title.image, (_sprite) =>
                {
                    tTitle.GetComponent<Image>().sprite = _sprite;
                });

                var tEntry = rootUI.transform.Find("BannerMenu/entry");
                tEntry.gameObject.SetActive(false);
                for (int i = 0; i < style_.bannerMenu.entries.Length; i++)
                {
                    var entry = style_.bannerMenu.entries[i];
                    var clone = GameObject.Instantiate(tEntry.gameObject, tEntry.parent);
                    clone.transform.localPosition = new Vector3(i * style_.bannerMenu.space, 0, 0);
                    clone.SetActive(true);
                    alignByAncor(clone.transform, entry.banner.anchor);
                    loadSpriteFromTheme(entry.banner.image, (_sprite) =>
                    {
                        clone.GetComponent<Image>().sprite = _sprite;
                        clone.transform.Find("__gaze__").GetComponent<Image>().sprite = _sprite;
                    });

                    var tEnter = clone.transform.Find("enter");
                    tEnter.gameObject.SetActive(false);
                    enterButtons.Add(tEnter);

                    clone.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach(var button in enterButtons)
                        {
                            button.gameObject.SetActive(button == tEnter);
                        }
                    });

                    tEnter.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach(var subject in entry.subjects)
                        {
                            publishSubject(subject);
                        }
                    });

                    //TODO 以摄像机为中心计算旋转角度
                }


            }
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

    }
}
