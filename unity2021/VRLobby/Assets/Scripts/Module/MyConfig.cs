
using System.Xml.Serialization;

namespace XTC.FMP.MOD.VRLobby.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class GazeUiElement : UiElement
        {
            [XmlAttribute("gazeImage")]
            public string gazeImage { get; set; }
        }


        public class BannerEntry
        {
            [XmlElement("Banner")]
            public GazeUiElement banner { get; set; } = new GazeUiElement();
            [XmlElement("Entry")]
            public GazeUiElement entry { get; set; } = new GazeUiElement();
            [XmlArray("OnSubjects"), XmlArrayItem("Subject")]
            public Subject[] onSubjects { get; set; } = new Subject[0];
            [XmlArray("OffSubjects"), XmlArrayItem("Subject")]
            public Subject[] offSubjects { get; set; } = new Subject[0];
        }

        public class BannerMenu
        {
            [XmlElement("Title")]
            public UiElement title { get; set; } = new UiElement();

            [XmlAttribute("distance")]
            public float distance { get; set; } = 2;

            [XmlAttribute("space")]
            public float space { get; set; } = 20;

            [XmlElement("BannerEntry")]
            public BannerEntry bannerEntry { get; set; } = new BannerEntry();
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";

            [XmlAttribute("active")]
            public string active { get; set; } = "";

            [XmlElement("BannerMenu")]
            public BannerMenu bannerMenu { get; set; } = new BannerMenu();
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

