using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Sahaya;

namespace Sahaya.data
{
      public abstract class SampleDataCommon : Sahaya.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

      /// <summary>
      /// Generic item data model.
      /// </summary>
      public class SampleDataItem : SampleDataCommon
      {
          public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
              : base(uniqueId, title, subtitle, imagePath, description)
          {
              this._content = content;
              this._group = group;
          }

          private string _content = string.Empty;
          public string Content
          {
              get { return this._content; }
              set { this.SetProperty(ref this._content, value); }
          }

          private SampleDataGroup _group;
          public SampleDataGroup Group
          {
              get { return this._group; }
              set { this.SetProperty(ref this._group, value); }
          }
      }

      /// <summary>
      /// Generic group data model.
      /// </summary>
      public class SampleDataGroup : SampleDataCommon
      {
          public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
              : base(uniqueId, title, subtitle, imagePath, description)
          {
              Items.CollectionChanged += ItemsCollectionChanged;
          }

          private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
          {
              // Provides a subset of the full items collection to bind to from a GroupedItemsPage
              // for two reasons: GridView will not virtualize large items collections, and it
              // improves the user experience when browsing through groups with large numbers of
              // items.
              //
              // A maximum of 12 items are displayed because it results in filled grid columns
              // whether there are 1, 2, 3, 4, or 6 rows displayed

              switch (e.Action)
              {
                  case NotifyCollectionChangedAction.Add:
                      if (e.NewStartingIndex < 12)
                      {
                          TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                          if (TopItems.Count > 12)
                          {
                              TopItems.RemoveAt(12);
                          }
                      }
                      break;
                  case NotifyCollectionChangedAction.Move:
                      if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                      {
                          TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                      }
                      else if (e.OldStartingIndex < 12)
                      {
                          TopItems.RemoveAt(e.OldStartingIndex);
                          TopItems.Add(Items[11]);
                      }
                      else if (e.NewStartingIndex < 12)
                      {
                          TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                          TopItems.RemoveAt(12);
                      }
                      break;
                  case NotifyCollectionChangedAction.Remove:
                      if (e.OldStartingIndex < 12)
                      {
                          TopItems.RemoveAt(e.OldStartingIndex);
                          if (Items.Count >= 12)
                          {
                              TopItems.Add(Items[11]);
                          }
                      }
                      break;
                  case NotifyCollectionChangedAction.Replace:
                      if (e.OldStartingIndex < 12)
                      {
                          TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                      }
                      break;
                  case NotifyCollectionChangedAction.Reset:
                      TopItems.Clear();
                      while (TopItems.Count < Items.Count && TopItems.Count < 12)
                      {
                          TopItems.Add(Items[TopItems.Count]);
                      }
                      break;
              }
          }

          private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
          public ObservableCollection<SampleDataItem> Items
          {
              get { return this._items; }
          }

          private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
          public ObservableCollection<SampleDataItem> TopItems
          {
              get { return this._topItem; }
          }
      }

      /// <summary>
      /// Creates a collection of groups and items with hard-coded content.
      /// 
      /// SampleDataSource initializes with placeholder data rather than live production
      /// data so that sample data is provided at both design-time and run-time.
      /// </summary>
      public sealed class SampleDataSource
      {
          private static SampleDataSource _sampleDataSource = new SampleDataSource();

          private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
          public ObservableCollection<SampleDataGroup> AllGroups
          {
              get { return this._allGroups; }
          }

          public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
          {
             
              return _sampleDataSource.AllGroups;
          }

          public static SampleDataGroup GetGroup(string uniqueId)
          {
              // Simple linear search is acceptable for small data sets
              var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
              if (matches.Count() == 1) return matches.First();
              return null;
          }

          public static SampleDataItem GetItem(string uniqueId)
          {
              // Simple linear search is acceptable for small data sets
              var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
              if (matches.Count() == 1) return matches.First();
              return null;
          }

          public SampleDataSource()
          {
              String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                          "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

              var group1 = new SampleDataGroup("Group-1",
                      "Trees",
                      "",
                      "images/group1.png",
                      "Trees provide essential structure and lay the framework for your landscape. Trees not only provide a focal point for your yard, but also offer shade as well as shelter for birds and wildlife. And although many trees tower far above the ground, there are varieties for virtually all landscape needs and wants. ");
                   
              group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                      "Neem Tree",
                      "Special Feature: Medicinal uses",
                      "images/neem.png",
                      "",
                      "Other names: It is popularly known as the miracle tree. It is known as Nimba in India. The Sanskrit name of Neem is Arishtha meaning the reliever of the sickness. Margosa tree \nDescription : It is a tall evergreen tree with the small bright green leaves. It is up to 100 feet tall. It blossoms in spring with the small white flowers. It has a straight trunk. Its bark is hard rough and scaly, fissured even in small trees. The colour of the bark is brown grayish. The leaves are alternate and consists of several leaflets with serrated edges. Its flowers are small and white in colour. The loive like edible fruit is oval, round and thin skinned. \nneem-treeOther Species : A. juss, A. azedarac are the other related species of Neem tree. A. juss, A. azedarac are the other related species of Neem tree. A. juss, A. azedarac are the other related species of Neem tree. \nLocation : Neem tree is found throughout India. It is a popular village tree. Although it is also widely grown in Ranthambore National Park, Bandhavgarh national Park, Mrugavani Naional Park, Bannerghata National Park, Sariska Wildlife Sanctuary and Guindy National Park. \nCultivation : Neem tree can easily be grown in the dry, stony, shallow and clayey soils. It needs very little water and plenty of sunlight. It grows slowly during the first year of planting. It can be propagated through the seeds and cuttings. Young neem tree can not tolerate excessive cold. . \nMedicinal uses : The indigenous people of Nilgiris consume the dried and powered tubulers of the terrestrial orchids as an energizing tonic. Neem also holds medicinal value. Each part of neem is used in the medicines. It has been used in Ayurvedic medicines for more than 4000 years. Neem oil extracted from its seeds is used in medicines, pest control and cosmetics etc. Its leaves are used in the treat Chickenpox.. According to the Hindus, it is believed that the Goddess of the chickenpox, Sithala lives in the Neem tree. Neem tea is usually taken to reduce the headache and fever. Its flowers are used to cure intestinal problems. Neem bark acts as an analgesic and can cure high fever as of malaria. Even the skin diseases can be cured from the Neem leaves. Indians even believe that the Neem can even purify diseases. \nOther uses : People in India use its twigs to brush their teeth. Neem is considered as the useful tree in rehabilitating the waste land areas. Neem seed pulp is useful for methane gas production. It is also useful as carbohydrate which is rich base for other industrial fermentations. Neem bark contains tannins which are used in tanning and dyeing. In south India its wood is used to make the furniture. The bark of the yields the fiber that is woven into ropes. Neem cake is widely used in India as fertilizer for sugarcane, vegetable and other cash crops. Many countries have been consistently growing the Neem tree against the global warming. The worldwide Neem Foundation has helped in making the people aware about the importance of neem and its uses globally. \nCultural Importance : One can find Neem in almost all the parts of India. It is said that planting Neem tree in the house is a ensured passage to heaven. Its leaves are stung on the main entrance to remain away from the evil spirits. Brides take bath in the water filled with the Neem leaves. Newly born babies are laid upon the Neem leaves to provide them with the protective aura. Neem gives out more oxygen than other trees. The neem tree is also connected with the Sun, in the story of Neembark 'The Sun in the Neem tree'. Neem is the wonder tree and finds mention in the number of ancient texts.\n\n Kingdom :	Plantae\nDivision	Magnoliophyta\nClass:	Magnoliopsida\nOrder :	Sapindales\nFamily :	Meliaceae\nGenus :	Azadirachta\nSpecies :	A. indica\nScientific Name :	Azadirachta indica\nFound In :	Ranthambore National Park, Bandhavgarh national Park, Mrugavani Naional Park, Bannerghata National Park, Sariska Wildlife sanctuary and Guindy National Park.",
                      group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                    "Teak Tree",
                    "Special Feature: Medicinal use ",
                    "images/teak.png",
                    " ",
              "Other names : Saka, Burma teak, Rangoon teak, moulmein teak, gia thi, jati sak, kyun, mai sak, rosawa and tekka are the other names used for the Teak tree. \nDescription : Teak is tall evergreen tree. It has yellowish blonde to reddish brown wood. It attains the height of about 30 meter. The fruit is a drupe. It has bluish to white flowers. It produces the large leaf similar to the tobacco leaf. The bark is whitish gray in colour. It is generally grown straight teak-treewith the uneven texture, medium lusture and the oily feel. The upper surface of the tree is rough to touch and the inner surface has hairs. The fruit is enclosed by the bladder like calyx, which is light brown, ribbed and papery. \nOther species : Tectona grandia, Tectona hamiltoniana, and Tectona philippinensis are the other related species of the Teak tree. \nLocation : Teak is well grown in all the parts of India. It is also found in the Gir National Park, Satpura National Park, Pench Tiger Reserve in India. \nCultivation methods : The new plants can also be propagated from cuttings. It is usually planted when the four to six weeks old. Plough the land thoroughly and level it. The best season to plant the teak is monsoon, most probably after the first shower. Carry out weeding operations regularly. Teal requires loamy soil rich in humus and having the right content of moisture with good drainage. It grows well in hilly and dry areas. It requires a dry tropical climate for its growth. It flowers in february and March.\nMedicinal uses : Teak also holds the medicinal value. The bark is bitter tonic and is considered useful in fever. It is also useful in headache and stomach problems. Digestion may be enhanced by the teak wood or bark. . \nOther uses : It is used in the furniture making, boat decks and for indoor flooring. It is widely used to make the doors and house windows. It is resistant to the attack of termites. Its wood contains scented oil which is the repellent to insects. The leaves yield the dye which is used to colour the clothes and edible. Teak is probably the best protected commercial species in the world. \n\nKingdom :	Plantae\nDivision	Magnoliophyta\nClass:	Magnoliopsida\nOrder :	Lamiales\nFamily :	Verbenaceae\nGenus :	Tectona\nScientific Name :	Tectona Grandis\nFound In :	Gir National Park, Satpura National Park, Pench Tiger Reserve",
                    group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-3",
                   "American persimmon",
                   "Special Feature: Attracts Birds",
                   "images/persimmon.png",
                   "",
                  "American persimmon is a tall shade tree that's sadly underused in gardens. It features dark green foliage that often develops yellow or red tones in fall. Older trees have distinctive bark that almost looks scaly, as though it's covered in small silvery plates.\nMale and female flowers appear on separate plants, the female trees produce an edible fruit if there's a male nearby for pollination. The fruits are also great for attracting birds.\nAmerican persimmon does best in full sun and moist, well-drained soil. But it tolerates drought fairly well.\n\nLight: Sun\nType: Tree\nHeight: 8 to 20 feet\nWidth: To 35 feet wide\nFoliage Color: Chartreuse/Gold\n Seasonal Features: Colorful Fall Foliage, Summer Bloom\nProblem Solvers: Good For Privacy\nSpecial Features: Attracts Birds\nZones: 4-9",
                   group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-4",
                      "American hornbeam",
                      "Seasonal Features: Colorful Fall Foliage, Spring Bloom, Winter Interest",
                      "images/hornbeam.png",
                      "",
                      "An adaptable tree sadly overlooked by gardeners, hornbeam is a slow-growing small tree with strong wood. In fall, the foliage turns shades of yellow, orange, and red; in winter, the fluted texture of the bark gives hornbeam one of its other common names: musclewood.\nHornbeam thrives in full sun or partial shade, and its small size makes it useful for growing in parking strips or other tight spaces. Native to areas of North America, it can be grown with a single trunk or a clump of smaller trunks; it develops a rounded shape.\n\nLight: Part Sun, Sun\nType: Tree\nHeight: Under 6 inches to 20 feet\nWidth: To 50 feet wide\nFoliage Color: Chartreuse/Gold\nSeasonal Features: Colorful Fall Foliage, Spring Bloom, Winter Interest\nProblem Solvers: Good For Privacy\nZones: 3-9",
                      group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-5",
                      "Anacacho orchid ",
                      "Special Features: Fragrance",
                      "images/orchid.png",
                      "",
                      "A drought-tolerant tree with fragrant flowers is a challenge to come by, but Anacacho orchid tree is up for the challenge. Native to Texas and New Mexico, this plant thrives in lean, fast-draining soil and is decorated with sweetly fragrant white flowers that resemble orchids in spring. It has an open, thin growth habit in shade but will form a denser canopy that can be useful for privacy if planted in full sun. It's an excellent tree for xeric landscapes and offers great deer resistance to boot.\n\nLight: Part Sun, Sun\nType: Tree\nHeight: Under 6 inches to 20 feet\nWidth: To 10 feet wide\nFlower Color: White\nSeasonal Features: Spring Bloom\nProblem Solvers: Deer Resistant, Drought Tolerant, Good For Privacy\nSpecial Features: Attracts Birds, Fragrance\nZones: 8-10",
                      group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-6",
                      "Aspen",
                      "Special Features: Attracts Birds, Low Maintenance",
                      "images/aspen.png",
                      "",
                      "Dancers in the garden, aspens are popular choices for fast-growing windbreaks, screens, and mass plantings. Their oval leaves flutter in the slightest breeze. These extremely cold-hardy trees can gain almost 5 feet in height per year. Avoid problems with their invasive roots and suckering by selecting species and varieties that won't run rampant. Enjoy the best fall color with the quaking aspen. The trees have a preference for moist, well-drained soil but they adapt to almost any soil.\n\nLight: Sun\nType: Tree\nHeight: From 8 to 20 feet\nWidth: To 30 feet wide\nSeasonal Features: Colorful Fall Foliage\nProblem Solvers: Drought Tolerant, Good For Privacy, Slope/Erosion Control\nSpecial Features: Attracts Birds, Low Maintenance\nZones: 2-8",
                      group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-7",
                    "Ash",
                    "Special Features: Attracts Birds",
                    "images/ash.png",
                    "",
                    "The magnificent shade tree that has it all: tolerance for difficult soils and conditions; spectacular purple, red, orange, or gold fall color; and a stately silhouette. Shapes range from broad-domed to narrow teardrop, but most ash varieties will require a large, open space to become the crowning glory of your landscape. Ashes are good choices for dry or alkaline soils.Light: Sun\nType: Tree\nHeight: 8 to 20 feet\nWidth: 30-50 feet wide\nSeasonal Features:Colorful Fall Foliage\nProblem Solvers: Drought Tolerant, Good For Privacy, Slope/Erosion Control\nSpecial Features: Attracts Birds\nZones: 2-9",
                    group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-8",
                  "Cedar",
                  "Special Features:Attracts Birds, Fragrance, Good for Containers, Low Maintenance",
                  "images/cedar.png",
                  "",
                  "Graceful sweeping branches and a natural pyramidal shape are the hallmarks of this exceptionally fragrant evergreen. If given plenty of space, cedars will grow to traffic-stopping perfection, to be especially appreciated in the winter landscape. Needle color ranges from yellow-tipped green to the silvery tones of the blue Atlas cedar. The deodar cedar has a Christmas tree shape up to 150 feet tall. All cedars are relatively problem free, but dislike wet feet and very cold winters.Light: Part Sun, Sun\nType: Tree\nHeight: From 8 to 20 feet\nWidth: To 100 feet wide\nSeasonal Features: Winter Interest\nProblem Solvers: Deer Resistant, Good For Privacy, Slope/Erosion Control\nSpecial Features: Attracts Birds, Fragrance, Good for Containers, Low Maintenance\nZones:6-9",
                  group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-9",
                   "Coconut Tree",
                   "Special Features: Low Maintenance",
                   "images/coconut.png",
                   "",
                   "Coconut Tree is the most recognizable palm in the world. This tree grows 60-100 feet tall and 20-30 feet wide with huge leaves to 15 feet long. Coconut fruits hang in clusters under and among leaf bases. Immature fruits range in color from green to yellow or red, but all eventually turn brown. This palm is extremely drought-tolerant and can withstand salty soils.\n's a versatile plant used to make copra (coconut meat), coconut oil, coir (a material often added to potting mixes), and coconut milk.\nLight: SunType: Tree\nHeight:8 to 20 feet\nWidth: 20-30 feet wide\nProblem Solvers:Drought Tolerant\nZones: 10-11",
                   group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-10",
                 "Bald cypress",
                 "Special Features: Low Maintenance",
                 "images/item10.png",
                 "",
                 "Bald cypress is an easy-to-grow North American native conifer that features feathery, soft, green needles and attractive peeling bark. Unlike many needled conifers, the needles turn a delightful shade of russet-red in autumn, then fall off the tree in winter revealing its delightful architectural shape. In spring, new needles emerge.\nBald cypress is wonderfully adaptable, growing well in any average or wet soil. This is one of the few trees that tolerates standing water. It grows best in full sun and moist, acidic soil, however.\nBald cypress is the official state tree of Louisiana.\nLight: Part Sun, Sun\nType: Tree\nHeight:8 to 20 feet\nWidth: To 30 feet wide\nSeasonal Features: Colorful Fall Foliage\nSpecial Features: Low Maintenance\nZones: 5-10",
              group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-11",
              "Beech",
              "Special Features: Attracts Birds, Good for Containers",
              "images/beech.png",
              "",
              "A versatile, handsome tree, the beech takes center stage in the garden come fall when leaves change to red, gold, orange, or brown. Beech trees stand proudly upright or bend and weep; jagged leaves vary from deep green to variegated rose, white, green, or bronzy-purple. For the best leaf color, plant beeches in full sun. The hardy American beech is a U.S. native with larger leaves and light gray bark.Light: Part Sun, Sun\nType: Tree\nHeight:From 8 to 20 feet\nWidth:35-45 feet wide\nFoliage Color:Blue/Green, Purple/Burgundy\nSeasonal Features: Colorful Fall Foliage, Spring Bloom, Winter Interest\nProblem Solvers: Slope/Erosion Control\nSpecial Features: Attracts Birds, Good for Containers\nZones: 5-8",
              group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-12",
            "Birch",
            "Special Features: Attracts Birds, Good for Containers",
            "images/birch.png",
            "",
            "One of the most elegant garden trees, birches make a graceful statement with open, airy branches and roughly textured trunks. They're especially dramatic when planted as an allee (in rows on either side of a path), in a grove, or near water where their impact is doubled in reflection. River birch is a U.S. native that's among the easiest to grow.\nLight:Part Sun, Sun\nType:Tree\nHeight:8 to 20 feet\nWidth:15-25 feet wide\nFoliage Color:Blue/Green\nSeasonal Features:Colorful Fall Foliage, Winter Interest\nProblem Solvers:Slope/Erosion Control\nSpecial Features:Attracts Birds, Good for Containers\nZones:2-7",
            group1));
              group1.Items.Add(new SampleDataItem("Group-1-Item-13",
           "Bradford pear",
           "Special Features: Attracts Birds, Good for Containers",
           "images/birch.png",
           "",
           "Snowy early-spring blossoms and a tall pyramidal shape make flowering pear the ideal lawn tree for home landscapes. It also tolerates urban conditions such as air pollution. Select smaller, narrower varieties such 'Chanticleer' and 'Valiant' for street-side tree planting. A bonus is the fall color; 'Redspire' is a good choice for deep purple-red fall foliage. The tiny fruits appeal to summer birds.\nLight:Part Sun, Sun\nType:Tree\nHeight:8 to 20 feet\nWidth:15-20 feet wide\nFlower Color:White\nSeasonal Features:Colorful Fall Foliage, Spring Bloom\nProblem Solvers:Drought Tolerant, Slope/Erosion Control\nSpecial Features:Attracts Birds, Good for Containers, Low Maintenance\nZones:5-8",
           group1));
              this.AllGroups.Add(group1);

              var group2 = new SampleDataGroup("Group-2",
                      "Shurbs",
                      " ",
                      "images/group2.png",
                      "Shrubs are a key foundation planting for many gardens. They offer structure and organizing points; many also supply year-round color, as well as food and shelter for wildlife.");
    group2.Items.Add(new SampleDataItem("Group-2-Item-1",
                      "Red Gem",
                      "Special Features:Cut Flowers",
                      "images/redgem.png",
                      "",
                      "Hailing from South Africa, Leucadendron 'Red Gem' has an unusual appearance that makes it right at home in a mixed shrub or perennial border in a mild-climate garden. The bushy shrub has green leaves with a section of red near the leaf tips. The leaves take on a bronze appearance in late fall, and striking red-and-yellow flowers decorate the tops of the stems in winter and spring. The flowers are excellent for cutting.\nLeucadendron 'Red Gem' grows well in sun and well-drained soil. It tolerates drought with ease after a strong root system is established.\nLight:Sun\nType:Shrub\nHeight:3 to 8 feet\nWidth:To 5 feet wide\nFlower Color:Red, Yellow\nSeasonal Features:Spring Bloom, Winter Bloom\nProblem Solvers:Drought Tolerant, Good For Privacy\nSpecial Features:Cut Flowers\nZones: 9-10",
                      group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                      "Arborvitae",
                      "Special Features: Attracts Birds, Good for Containers, Low Maintenance",
                      "images/arborvitae.png",
                      "",
                      "Arborvitae will flourish where no other evergreen does, spreading a lush screen of fan-like foliage that provides privacy and gives winter shelter to the birds. For garden sculptors, arborvitae offers just the right texture and growth habit for topiaries. Many dwarf varieties are available as fillers and vertical accents for smaller gardens. Arborvitae prospers in deeply cultivated, moist and fertile soil in full sun.\nLight:Sun\nType:Shrub\nHeight:8 to 20 feet\nWidth:To 15 feet wide, depending on type\nFlower Color:Yellow\nFoliage Color:Blue/Green\nSeasonal Features:Spring Bloom, Winter Interest\nProblem Solvers:Deer Resistant, Drought Tolerant, Good For Privacy, Groundcover, Slope/Erosion Control\nSpecial Features:Attracts Birds, Good for Containers, Low Maintenance\nZones:5-9",
                      group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                      "Banana shrub",
                      "Special Features:Fragrance, Low Maintenance",
                      "images/banana.png",
                      "",
                      "A grand Southern lady, banana shrub is a member of the magnolia family. Its lovely springtime flowers resemble magnolia blooms but have a bold banana fragrance. The evergreen shrub's flush of flowers in spring is followed by sporadic flowering through summer. Plant this lovely shrub in beds or borders, or use it as a fragrant hedge. It tolerates pruning well and can be maintained at 4-5 feet tall. Water banana shrub regularly after planting. After it is established, it tolerates drought with ease.\nLight:Part Sun, Sun\nType:Shrub\nHeight:Under 6 inches to 20 feet\nWidth:To 10 feet wide\nFlower Color:Yellow\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Spring Bloom, Summer Bloom\nProblem Solvers:Drought Tolerant\nSpecial Features:Fragrance, Low Maintenance\nZones:8-10",
                      group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "Barberry",
                    "Special Features:Attracts Birds, Cut Flowers, Low Maintenance",
                    "images/barberry.png",
                    "",
                    "Barberry paints the landscape with arching, fine-textured branches of purple-red or chartreuse foliage. In fall, leaves brighten to reddish orange and spikes of red berries appear like sparklers as the foliage drops. The mounding habit of barberries makes for graceful hedging and barriers, and the thorns protect privacy.\nJapanese barberry is considered an invasive plant in the Eastern U.S. and the species is banned from cultivation in some places, so check local restrictions before planting.\nLight:Part Sun, Sun\nType:Shrub\nHeight:3 to 8 feet\nWidth:To 7 feet wide\nFlower Color:Yellow\nFoliage Color:Chartreuse/Gold, Purple/Burgundy\nSeasonal Features:Colorful Fall Foliage, Spring Bloom, Winter Interest\nProblem Solvers:Deer Resistant, Drought Tolerant, Good For Privacy, Groundcover, Slope/Erosion Control\nSpecial Features:Attracts Birds, Cut Flowers, Low Maintenance\nZones:3-9",
              group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-5",
                  "Beautyberry",
                  "Special Features:Attracts Birds,Low Maintenance",
                  "images/beautyberry.png",
                  "",
                  "Beautyberry is one shrub that's really earned its common name. In fall, the plant becomes a showstopper thanks to its clusters of small violet-purple fruits. The bright color stands out, especially after the plant loses its leaves. The fruits develop from summer's clusters of small pink flowers and may attract several species of birds to your yard.\nBeautyberry blooms on fresh growth, so if you need to prune it, the best time to do so is late winter or early spring. In the coldest areas of its range, it's sometimes grown like a perennial in that the stems die back to the ground every year and are replaced by new shoots in the spring.\nThis adaptable shrub blooms well in full sun or part shade and is relatively drought tolerant.\nLight:Part Sun, Sun\nType:Shrub\nHeight:From 3 to 20 feet\nWidth:From 4 to 8 feet wide\nFlower Color:Pink\nSeasonal Features:Summer Bloom\nSpecial Features:Attracts Birds, Low Maintenance\nZones:6-8",
              group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-6",
                 "Boxwood",
                 "Special Features:Fragrance, Good for Containers",
                 "images/boxwood.png",
                 "",
                 "An evergreen shrub ideal for sculpting, boxwood can take the shape of a neat mound or grow into small green clouds of foliage if left unmanicured. It's one of the most popular choices for garden topiaries. This fragrant shrub is frequently used as an outliner and definer around garden beds and path; it forms graceful short hedges. Garden neat freaks will want to wield the pruning shears frequently to keep boxwood in bounds. Provide a well-drained soil for boxwood to prevent problems with root rot.\nLight:Part Sun, Sun\nType:Shrub\nHeight:8 to 20 feet\nWidth:To 15 feet wide\nFlower Color:White\nFoliage Color:Blue/Green\nSeasonal Features:Spring Bloom, Winter Interest\nProblem Solvers:Deer Resistant, Good For Privacy, Groundcover, Slope/Erosion Control\nSpecial Features:Fragrance, Good for Containers\nZones:5-8",
                 group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-7",
               "Bush Poppy",
               "Special Features:Attracts Birds, Cut Flowers, Low Maintenance",
               "images/bushpoppy.png",
               "",
               "A tough shrub for challenging sites, bush poppy adds a sunny splash of yellow to dry, quick-draining planting areas. Covered with 2-inch-wide flowers from March through June, it is native to California and will quickly reach 6 feet tall in about two years. When not blooming, bush poppy's gray-green leaves provide pleasing texture and form in the garden. Plant bush poppy in full sun or part shade in well-drained soil. It does not tolerate clay well. Do not fertilize bush poppy. It grows and flowers best when it is lean on nutrients.\nLight:Part Sun, Sun\nType:Shrub\nHeight:3 to 8 feet\nWidth:To 6 feet tall\nFlower Color:Yellow\nSeasonal Features:Spring Bloom\nProblem Solvers:Drought Tolerant, Good For Privacy\nSpecial Features:Attracts Birds, Cut Flowers, Low Maintenance\nZones:9-11",
               group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-8",
             "Butterfly bush",
             "Special Features:Attracts Birds, Cut Flowers, Fragrance, Low Maintenance",
              "images/butterflybush.png",
             "",
             "Drenching the air with a fruity scent, butterfly bush's flower spikes are an irresistible lure to butterflies and hummingbirds all summer long. The plants have an arching habit that's appealing especially as a background in informal flower borders. In warmer climates, butterfly bushes soon grow into trees and develop rugged trunks that peel.\nTo nurture butterfly bush through cold Northern winters, spread mulch up to 6 inches deep around the trunk. Plants will die down, but resprout in late spring. Prune to the ground to encourage new growth and a more fountainlike shape. Avoid fertilizing butterfly bush; extra-fertile soil fosters leafy growth rather than flower spikes. Remove spent flower spikes to encourage new shoots and flower buds.\nNote: Butterfly bush can be an invasive pest in some areas; check local restrictions before planting it.\nLight:Sun\nType:Shrub\nHeight:8 to 20 feet\nWidth:To 8 feet wide\nFlower Color:Blue, Pink, Red, White\nFoliage Color:Chartreuse/Gold, Gray/Silver\nSeasonal Features:Summer Bloom\nProblem Solvers:Deer Resistant, Drought Tolerant, Slope/Erosion Control\nSpecial Features:Attracts Birds, Cut Flowers, Fragrance, Low Maintenance\nZones:5-10",
              group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-9",
           "Bush anemone",
           "Special Features:Cut Flowers, Low Maintenance",
           "images/bushanemone.png",
           "",
           "The sparkling white flowers of bush anemone will cool down the hottest afternoon. An evergreen shrub native to California, it is a great plant for the back of a perennial border or an informal hedge. Bush anemone grows well in full sun or part shade and tolerates a range of soil conditions but does best in well-drained soil. It thrives on neglect: do not fertilize, and water only during periods of extended drought.\nLight:Part Sun, Sun\nType:Shrub\nHeight:3 to 8 feet\nWidth:To 3 feet wide\nFlower Color:White\nSeasonal Features:Summer Bloom, Winter Interest\nProblem Solvers:Drought Tolerant, Good For Privacy\nSpecial Features:Cut Flowers, Low Maintenance\nZones:9-10",
           group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-10",
          "Camellia",
          "Special Features:Cut Flowers, Good for Containers",
          "images/camellia.png",
          "",
          "The waxy, perfectly shaped blooms of camellias plant cheer late winter landscapes, opening against dark, glossy green leaves. Thousands of double camellia hybrids offer a large palette of colors from snowy white and bicolors to the deepest coral-red. The upright plants develop into small trees in warm climates. A camellia looks stunning when espaliered against a warm wall; avoid full sun situations to prevent summer-scorched leaves.\nLight:Part Sun, Shade\nType:Shrub\nHeight:8 to 20 feet\nWidth:To 20 feet wide\nFlower Color:Orange, Pink, Red, White\nFoliage Color:Blue/Green, Chartreuse/Gold\nSeasonal Features:Fall Bloom, Spring Bloom, Summer Bloom, Winter Bloom, Winter Interest\nProblem Solvers:Good For Privacy\nSpecial Features:Cut Flowers, Good for Containers\nZones:6-9",
          group2));
              group2.Items.Add(new SampleDataItem("Group-2-Item-11",
        "Carolina Allspice",
        "Special Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance",
        "images/carolina.png",
        "",
        "A wonderful, easy-to-grow shrub, Carolina allspice features strongly fragrant dark red flowers in early summer. The show doesn't stop there; the leaves often turn a nice shade of yellow in the fall.\nCarolina allspice is largely left alone by deer, probably thanks to its clove-scent foliage. The shrub thrives in full sun or part shade and in moist, well-drained soil. It's native to areas of North America.\nType:Shrub\nHeight:3 to 8 feet\nWidth:To 10 feet wide\nFlower Color:Red\nSeasonal Features:Colorful Fall Foliage, Summer Bloom\nProblem Solvers:Deer Resistant, Slope/Erosion Control\nSpecial Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance\nZones:5-9",
        group2));
              this.AllGroups.Add(group2);

              var group3 = new SampleDataGroup("Group-3",
                      "Herbs",
                      "",
                      "images/group3.png",
                      "Growing herbs is a simple way to add edible plants to your garden. Most herbs are very versatile, and grow well in the ground or in containers. Herbs, which generally are annuals except in very warm climates, make a great addition to a traditional flower garden, and are also a pretty, practical accent to windowboxes or containers near a grill or outside a kitchen door. ");
              group3.Items.Add(new SampleDataItem("Group-3-Item-1",
                      "Caraway",
                      "Special Features:Cut Flowers, Good for Containers, Low Maintenance",
                      "images/caraway.png",
                      "",
                      "This biennial develops ferny foliage its first year in the garden and bears white flowers and seeds the second year. The seeds are most commonly used to flavor rye and other breads, but all parts of the plant are edible. Caraway prefers a sunny location with rich, well-drained soil. Although the plant tolerates drought, don't let the soil dry out completely.\nLight:Part Sun, Sun\nType:Herb\nHeight:1 to 3 feet\nWidth:To 18 inches wide\nFlower Color:White\nSeasonal Features:Summer Bloom\nProblem Solvers:Drought Tolerant\nSpecial Features:Cut Flowers, Good for Containers, Low Maintenance\nZones:5-8",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-2",
                      "Aloe vera",
                      "Special Features:Good for Containers, Low Maintenance",
                      "images/aloevera.png",
                      "",
                      "The spiky green foliage of aloe vera is splotched in white and contains a gel-like sap often used to soothe burns and moisturize skin. This succulent perennial herb is at home in frost-free, sunny, well-drained sites. Native to hot, dry regions of Africa, it has been traced to early Egypt, where it was used for its healing properties. Aloe makes a great houseplant, especially in colder Zones where it cannot be grown outdoors all year. Aloe vera is also sometimes called Barbados aloe and true aloe.\nLight:Part Sun, Sun\nType:Herb, Houseplant\nHeight:1 to 3 feet\nWidth:6-36 inches wide\nFlower Color:Yellow\nSeasonal Features:Summer Bloom\nProblem Solvers:Drought Tolerant\nSpecial Features:Good for Containers, Low Maintenance\nZones:9-11",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-3",
                      "Bayberry",
                      "Special Features:Attracts Birds, Fragrance, Low Maintenance",
                      "images/bayberry.png",
                      "",
                      "Bayberry forms a beautiful semi-evergreen shrub that tolerates either wet or dry soils. The shrub also withstands salt spray, making it a good choice for coastal landscapes. Plants gradually spread from underground suckers, eventually forming a thicket. Pruning is rarely necessary.\nBayberry has long been prized for its fragrant, waxy gray berries, which can be used to make candles. Plants are either male or female; to ensure berry production, plant several shrubs in the same landscape. The berries are also attractive to a wide range of songbirds.\nLight:Part Sun, Sun\nType:Herb, Shrub\nHeight:3 to 8 feet\nWidth:To 8 feet wide\nSeasonal Features:Spring Bloom\nProblem Solvers:Drought Tolerant, Slope/Erosion Control\nSpecial Features:Attracts Birds, Fragrance, Low Maintenance\nZones:3-7 ",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-4",
                      "Catnip",
                      "Special Features:Attracts Birds, Low Maintenance",
                      "images/catnip.png",
                      "",
                     "Catnip is an easy-to-grow perennial grown primarily for its fragrant foliage that is extremely attractive to cats. A vigorous herb, catnip can be grown indoors on a sunny windowsill or in a bright location outdoors. As with many mints, it can become invasive. Plant it in a location where it is easily controlled. And remove the flower heads before they mature and set seeds. Harvest catnip leaves at any time as a treat for your favorite feline. You also can dry the leaves and stuff them into kitty toys. The aromatic foliage also repels mosquitoes.\nLight:Part Sun, Sun\nType:Herb\nHeight:1 to 3 feet\nWidth:18 inches wide\nFlower Color:White\nSeasonal Features:Summer Bloom\nProblem Solvers:\nDrought Tolerant\nSpecial Features:Attracts Birds, Low Maintenance\nZones:3-9",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-5",
                      "Chervil",
                      "Special Features:Attracts Birds, Good for Containers, Low Maintenance",
                      "images/chervil.png",
                      "",
                      "Punch up the flavor of springtime dishes with the low-calorie, big taste of chervil. This fuss-free herb thrives in garden beds or containers, growing easily from seed. Snip chervil to give an herbal boost to salmon, asparagus, new potatoes, cream sauces, and baby lettuce salads. Leaves blend a sweet, grassy taste with a hint of licorice. Chervil prefers moist soil and shaded roots. Plants don't transplant well; sow seeds where you want them to grow. Scatter seeds in beds or containers several times throughout the growing season for continuous harvest. In the garden, let a few flower stalks set and drop seed to enjoy continued chervil crops.\nLight:Part Sun, Sun\nType:Annual, Herb\nHeight:1 to 3 feet\nWidth:1 foot wide\nFlower Color:White\nSeasonal Features:Summer Bloom\nSpecial Features:Attracts Birds, Good for Containers, Low Maintenance",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-6",
                      "Coriander",
                      "Special Features:Attracts Birds, Fragrance, Good for Containers, Low Maintenance",
                      "images/coriander.png",
                      "",
                      "With bright green, fern-textured stems, cilantro holds its own in beds or pots, forming a clump of sturdy, flavorful stems. Every part of cilantro promises a taste treat: spicy leaves, pungent seeds (known as coriander), and tangy roots. Most gardeners grow cilantro for the foliage, which boasts a citrusy bite that enlivens Mexican and Thai cooking. You might see this herb called Chinese parsley.\nOnce flowers form, leaf flavor disappears. Pinch plants frequently to keep flowers at bay. Cilantro tends to bloom as summer heat settles in; growing plants in partial shade and adding mulch can stave off flower shoots -- but not indefinitely. To ensure a season-long supply of leaves, sow seeds every 2-4 weeks. If plants set seed, dry seeds for use as coriander, and save a few for sowing. Allow flowers to drop seeds in the garden and you may be rewarded with a second crop.\nLight:Part Sun, Sun\nType:Annual, Herb\nHeight:Under 6 inches to 8 feet\nWidth:4-10 inches wide\nFlower Color:Blue, Pink, White\nSeasonal Features:Summer Bloom\nProblem Solvers:Deer Resistant\nSpecial Features:Attracts Birds, Fragrance, Good for Containers, Low Maintenance",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-7",
                      "Chamomile",
                      "Special Features:Attracts Birds, Fragrance, Good for Containers, Low Maintenance",
                      "images/chamomile.png",
                      "",
                      "Chamomile's dainty daisylike blooms glisten when dew-spangled and glow in moonlight. Carpet a garden path or patio with Roman chamomile, a flowering groundcover that releases a delicate fragrance when crushed underfoot. Use this herbal groundcover in the garden to edge beds with a feathery, fast-spreading quilt or to cascade artfully over the rim of containers. German chamomile is a bushy beauty that's a favorite among bees and butterflies. Tucked into flower beds, it offers season-long color. Chamomile blooms brew a soothing tea. Toss fresh blossoms over salad, or use fresh or dried leaves to season butter, cream sauce, or sour cream.\nLight:Part Sun, Sun\nType:Annual, Herb, Perennial\nHeight:From 1 to 8 feet\nWidth:3-18 inches wide\nFlower Color:White\nSeasonal Features:Spring Bloom, Summer Bloom\nProblem Solvers:Deer Resistant, Drought Tolerant, Groundcover, Slope/Erosion Control\nSpecial Features:Attracts Birds, Fragrance, Good for Containers, Low Maintenance\nZones:4-9",
                      group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-8",
                    "Comfrey",
                    "Special Features:Attracts Birds, Cut Flowers, Good for Containers, Low Maintenance",
                    "images/comfrey.png",
                    "",
                    "Comfrey leaves are full of nutrients that make a natural high-potassium fertilizer or addition to compost. This perennial herb sends down deep roots that pull nutrients into the plant's large, hairy leaves. It grows best in moist sites high in organic matter. Common comfrey, Symphytum officinale, is a vigorous plant that can grow up to 4 feet tall. The plant spreads by rhizomes and can become invasive.\nType:Herb\nHeight:3 to 8 feet\nWidth:To 4 feet wide\nFlower Color:Blue, Purple, White\nSeasonal Features:Spring Bloom\nProblem Solvers:Deer Resistant\nSpecial Features:Attracts Birds, Cut Flowers, Good for Containers, Low Maintenance\nZones:4-9",
                    group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-9",
                    "Cuban oregano",
                    "Special Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance",
                    "images/cuban.png",
                    "",
                    "Cuban oregano could be called an herbal smorgasbord. Other common names for it include Mexican mint, Spanish thyme, and Indian mint -- an indication of its complex flavor. Cuban oregano has fuzzy succulent leaves on a plant that grows 12-18 inches tall and wide. It doesn’t survive freezing temperatures, but it is easy to start from cuttings. Plants can be taken indoors over winter and treated as houseplants. Cuban oregano is not a true mint, but rather is more closely related to Swedish ivy.\nLight:Part Sun, Sun\nType:Herb\nHeight:1 to 3 feet\nWidth:To 18 inches wide\nFlower Color:Blue, Purple, White\nSeasonal Features:Summer Bloom\nProblem Solvers:Drought Tolerant\nSpecial Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance\nZones:9-11",
                    group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-10",
                  "Epazote",
                  "Special Features:Good for Containers, Low Maintenance",
                  "images/epazote.png",
                  "",
                  "Epazote is a pungent tender perennial most commonly used in Mexican cooking. Use the leaves fresh or dried in bean dishes and soups. Epazote blends well with oregano, cumin, and chiles, but on its own it has a strong flavor that some compare to kerosene.\nMature plants grow 2-3 feet tall. Epazote prefers a dry, sunny site, but isn't particular about growing conditions. In fact, it readily spreads throughout the garden unless you contain it and remove flower stalks before they set seed.\nLight:Part Sun, Sun\nType:Herb\nHeight:1 to 3 feet\nWidth:To 3 feet wide\nProblem Solvers:Drought Tolerant\nSpecial Features:Good for Containers, Low Maintenance\nZones:8-11",
                  group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-11",
                 "Ginger",
                 "Special Features:Good for Containers",
                 "images/ginger.png",
                 "",
                 "Common cooking ginger is a tropical plant that can be grown outdoors year-round in Zones 8-11, or in a container to bring indoors over winter. Ginger prefers moist soil and part shade. If you take the plant indoors over winter, reduce the amount of moisture and light to slow growth. You can start plants from gingerroot (actually rhizomes) sold in grocery stores. The plant has little ornamental value, so it's not often sold in nurseries.\nLight:Part Sun\nType:Herb\nHeight:3 to 8 feet\nWidth:To 3 feet wide\nSpecial Features:Good for Containers\nZones:8-11",
                 group3));
              group3.Items.Add(new SampleDataItem("Group-3-Item-12",
                 "Horehound",
                 "Special Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance",
                 "images/horehound.png",
                 "",
                 "Horehound is a hardy member of the mint family. It has fuzzy gray-green foliage and small white flowers. Like mint, this plant can become invasive. Horehound is not fussy about growing conditions but prefers full sun and good drainage. Neither deer nor rabbits eat horehound unless they are extremely hungry. Plant the herb to deter these pests if they are a problem in your neighborhood. Horehound has traditionally been used as a cough suppressant or to make candy.\nType:Herb, Perennial\nHeight:1 to 3 feet\nWidth:2-3 feet wide\nFlower Color:White\nFoliage Color:Gray/Silver\nSeasonal Features:Summer Bloom\nProblem Solvers:Deer Resistant, Drought Tolerant\nSpecial Features:Cut Flowers, Fragrance, Good for Containers, Low Maintenance\nZones:3-9",
                 group3));
              this.AllGroups.Add(group3);

              var group4 = new SampleDataGroup("Group-4",
                      "Vegetable Plants",
                      "",
                      "images/group4.png",
                      "Growing and harvesting one's own vegetables is one of the most satisfying gardening experiences. But even if you don't want to transform a whole section of your landscape into a vegetable garden, you can grow quite a few vegetable types in small sections, in a container, or even interspersed with non-edibles.");
              group4.Items.Add(new SampleDataItem("Group-4-Item-1",
                      "Bean",
                      "Height:From 1 to 20 feet",
                      "images/beam.png",
                      "",
                      "Snap beans, also called green beans, are one of those must-have vegetables in the garden. They're easy to grow, are bothered by few pests, and if you choose a pole type, they take up hardly any square footage in the landscape. Or get creative and grow pole beans on fences or any other upright support\nBeans come many colors, shapes, and sizes. Pods may be green, yellow, purple, or speckled. The plants range in size from 2 feet tall for bush types to pole types that may climb to 12 feet. A bean harvested when young, before the seeds fully develop, is called a snap bean. Once the seeds have reached full size, but pods have not turned brown, it's called a shelling bean. After the pod dries and seeds mature, it's called a dried bean.\nLight:Sun\nType:Vegetable\nHeight:From 1 to 20 feet\nWidth:18-30 inches wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-2",
                      "Beet Root",
                      "Height:Under 6 inches to 3 feet",
                      "images/beatroot.png",
                      "",
                      "This old-fashioned favorite is becoming trendy once again. Use beetroots fresh, steamed, or roasted. At room temperature, beetroot is great in salads. It's also a favorite for pickling and canning. Although beetroots are usually red, they may also be yellow, pink, or stripped, creating a beautiful effect.\nThe leaves of beets are also prized. Usually, leaves are green with veins that match the root color, though some produce reddish-purple leaves. Tender young beet greens can be added to salads. When they're larger, they're usually steamed, sauted, stir-fried, or cooked. They're especially appreciated in the South, where they're \"cooked down\" with ham or bacon either solo or combined with other greens, such as mustard greens or collard greens.\nLight:Part Sun, Sun\nType:Vegetable\nHeight:Under 6 inches to 3 feet\nWidth:2-8 inches wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-3",
                      "Cabbage",
                      "Height:1 to 3 feet",
                      "images/cabbage.png",
                      "",
                      "Grow cabbage in your garden, and you're sure to gain a new appreciation of this ethnic favorite. Cabbage, after all, is a classic vegetable that's been a staple in Western diets for hundreds of years.\nIf you've ever endured eating overcooked boiled cabbage, you'll enjoy finding better ways to use the tender, homegrown version, especially if you experiment with the many interesting varieties available. There are early, midseason, and late varieties; round, conical, or flat-head types; smooth leaves or savoyed (crinkled) foliage; and colors ranging from yellow green to blue green, deep green, or purplish red. Each has a distinct flavor, with the red types being among the most sweet.\nUse homegrown cabbage fresh. Shred and add by the handful to mixed salads. When it's young and tender, it has a more mild flavor. Use it in classic or innovative slaws, too, and you'll find them a treat. Try stuffing large leaves. And, of course, you can always cook them, pickle them, or even make sauerkraut.\nLight:Part Sun, Sun\nType:Vegetable\nHeight:1 to 3 feet\nWidth:10-30 inches wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-4",
                      "Carrot",
                      "Height:1 to 3 feet",
                      "images/carrot.png",
                      "",
                      "Who knew you could have so much fun growing carrots? You can always grow carrots like the traditional, large, orange-root types, but also have fun with the many different types of carrots now available for gardeners to grow from seed.\nClassic orange-root carrots have been joined by new varieties in a rainbow of colors, ranging from red to white, yellow, and purple. They also come in a variety of shapes, including small, almost round, very large, and more cylindrical.\nCarrots are loaded with vitamin A and beta-carotene, both known as antioxidants and cancer fighters. Use carrots raw in salads, or explore their uses in Indian salads. The juice from carrots is the health-buff's staple. And they are, of course, fabulous in soups and stews or as a side dish. Cooking carrots makes the calcium in them more available, another nutritional bonus. \nLight:Part Sun, Sun\nType:Vegetable\nHeight:1 to 3 feet\nWidth:6-15 inches wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-5",
                      "Cauliflower",
                      "Height:From 1 to 8 feet",
                      "images/cauliflower.png",
                      "",
                      "Cauliflower is trickier to grow in some climates than its cousin, broccoli, but the effort is well worth it. And the reward of harvesting a large, attractive head from your garden will give a great sense of satisfaction.\nPlants grow best in cool (below 70 degrees F) weather. In most locations it's best to plant it 90 days before the first fall frost so heads will mature during cool weather. Traditional white-headed cauliflowers required gardeners to tie leaves over the developing head to ensure mild flavor and development of snow-white curds. Some newer varieties have outer leaves that naturally cover the head, eliminating the need for blanching. Varieties that develop colored heads of orange, purple, or chartreuse also need no blanching.\nLight:Part Sun, Sun\nType:Vegetable\nHeight:From 1 to 8 feet\nWidth:8-30 inches wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-6",
                      "Corn",
                      "Height:From 3 to 20 feet",
                      "images/corn.png",
                      "",
                      "Few things say summer like sweet corn, picked just minutes before eating. Sweet corn starts converting its sugars to starch the second you pick it, so it's hard to find sweet corn more tasty than that from your backyard.\nSweet corn takes space. It's essential to plant a number of rows (more is better) because the ears are wind pollinated and they need the critical mass for best production. For this reason, it's most efficient to plant corn in a block of short rows or hills rather than in a few long rows. Most stalks produce just one or two ears of corn, so plant plenty!\nAnd do what the professionals do: Plant early-, mid-, and late-season varieties to ensure the longest season of harvest, several weeks in late summer. Choose from standard sugary (su), sugar-enhanced (se), and supersweet (sh2) varieties with yellow, white, or bicolor kernels.\nLight:Sun\nType:Vegetable\nHeight:From 3 to 20 feet\nWidth:1-3 feet wide",
                      group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-7",
                    "Cucumber",
                    "Height:From 1 to 8 feet",
                    "images/cucumber.png",
                    "",
                    "Cucumbers are easy to grow, and just one plant will produce armloads of the crunchy, refreshing fruits. Use them to make cooling salads all through the hot summer. Add homegrown cucumbers to mixed leafy salads or mixed chopped vegetable salads. And try your hand at pickles, either the super easy refrigerator type or the canned type that Grandma used to make.\nCucumbers are grouped as slicers or picklers. All slicers are long and thin and are best eaten fresh. Picklers, which are shorter with more pronounced spines or bumps on their skin, are most often used preserved as pickles, but can be eaten fresh, too. Bush varieties produce vines only several feet long and are suited to growing in containers. Growing cucumbers on trellises and along fences allows for efficient use of space. Keep cucumbers watered well to avoid moisture stress, which can lead to bitterness.\nLight:Sun\nType:Vegetable\nHeight:From 1 to 8 feet\nWidth:1-2 feet wide",
                    group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-8",
                   "Eggplant",
                   "Height:1 to 3 feet",
                   "images/eggplant.png",
                   "",
                   "Eggplant is not only a wonderfully international food, it's also versatile. Add it to soups and pasta dishes or use it as the basis for hearty meatless casseroles and other entrees. Vegetarians value its rich, deep flavor as an excellent substitute for meat.\nEggplant originated in India, where it is used in many dishes, such as bengan bartha. The vegetable is also the basis for Greek moussaka, French ratatouille, and Italian caponata. The elegantly shaped, often gorgeously colored, glossy fruits vary from large oval teardrop-shape to small round grape size. And the mature color can range from white to purple, pink, striped, green, or orange. The plant needs heat and humidity to grow well. Fruit fails to set at temperatures below 65 degrees Fahrenheit.\nLight:Sun\nType:Vegetable\nHeight:1 to 3 feet\nWidth:1-2 feet wide\nFlower Color:Blue\nSeasonal Features:Summer Bloom",
                   group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-9",
                 "Fennel",
                 "Height:3 to 8 feet",
                 "images/fennel.png",
                 "",
                 "Avid cooks will enjoy growing fennel. The bulb, the feathery foliage, and even the seeds are excellent for European-inspired cooking.\nThe bulb and stems have an anise (licoricelike) flavor that adds interest to raw salads or vegetable appetizers served with a dip. The leaves are also excellent in salads or served snipped atop fish or chicken. And the seed adds a distinct flavor to Southern Italian-inspired red sauces.\nFlorence fennel, also called bulb fennel, differs from the perennial herb also called fennel in that Florence fennel forms a swollen base at ground level. For best bulb flavor, mound mulch around the base of the plant when bulbs reach 2 inches in diameter.\nLight:Sun\nType:Vegetable\nHeight:3 to 8 feet\nWidth:12-16 inches wide",
                 group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-10",
                "Okra",
                "Height:From 1 to 20 feet",
                "images/okra.png",
                "",
                "If you love okra, chances are you're from the South. This mainstay of Southern cooking is most commonly eaten breaded and fried or in gumbo, where its thick, viscous texture adds body and flavor to the regional favorite.\nNot surprisingly, this Southern favorite thrives in hot weather and warm soil. Although it's great fried or in gumbo, it can also be steamed, boiled, baked, grilled, or pickled. Okra is drought-tolerant, although it needs moisture during flowering and pod set.\nLight:Sun\nType:Vegetable\nHeight:From 1 to 20 feet\nWidth:1-3 feet wide",
                group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-11",
               "Onion",
               "Height:Under 6 inches to 3 feet",
               "images/onion.png",
               "",
               "Almost everyone loves onions. And when you grow them yourself, you get the most tender, sweet ones possible. The sweetest onions don't last long in storage, but because of their mild flavor they're great raw in salads or as a topper for grilled or cooked dishes. Storage onions are more pungent but develop a sweeter flavor when cooked.\nGreen onions, also called scallions or spring onions, are just immature bulbing onions that are harvested early. Leave them in the ground and they'll develop into regular onions.\nShallots are an onion relative with mild flavor and smaller bulbs. To ensure formation of large bulbs, plant shallots early in the season and grow the correct type for your area. Grow long-day types in the North and short-day types in the South, or plant intermediate-day types anywhere. Start onions from seed, transplants, or \"sets\" -- bundles of tiny immature onions.\nLight:Sun\nType:Vegetable\nHeight:Under 6 inches to 3 feet\nWidth:1-5 inches wide",
               group4));
              group4.Items.Add(new SampleDataItem("Group-4-Item-12",
             "Potato",
             "Height:1 to 3 feet",
             "images/potato.png",
             "",
             "Tender potatoes, harvested from your backyard, and then boiled and served with plenty of butter, are nothing short of heavenly. Growing potatoes is especially rewarding because there are so many new varieties. Try delicious fingerling and other potatoes, which often come in a rainbow of colors. Skin colors include red, white, blue, tan, and brown, and flesh colors include traditional white as well as yellow, red, blue and bicolors. Pick them small for the most delicate garden treat. Let them get larger if you want to mash or store them.\nPotatoes are usually grown from pieces of tuber, called sets or seed potatoes, rather than true seed. Plant them two to four weeks before the last spring frost. After sprouts emerge, mound soil around the stems to shade developing tubers from sun. Exposed tubers turn green, bitter, and mildly toxic (cut out any green portions before serving.)\nLight:Sun\nType:Vegetable\nHeight:1 to 3 feet\nWidth:1-2 feet wide",
             group4));
              this.AllGroups.Add(group4);

              var group5 = new SampleDataGroup("Group-5",
                      "House Plants",
                      "",
                      "images/group5.png",
                      "Growing houseplants is a wonderful way to add attractive foliage and flowers to indoor spaces. There's a houseplant for every living space, from small-scale terrariums to miniature trees. Every type of houseplant has particular growing requirements as well as preferences for sun and moisture.");
              group5.Items.Add(new SampleDataItem("Group-5-Item-1",
                      "Amaryllis",
                      "Special Features:Low Maintenance",
                      "images/amaryllis.png",
                      "",
                      "Amaryllis is an easy bulb to grow. Its enormous cluster of trumpet-shape blooms may require staking to keep them upright, but blooms may last for up to 6 weeks. Keep the plant cool (60-65 degrees F) while in bloom but slightly warmer at other times when it is actively growing. It needs bright light and evenly moist soil, except when it is dormant. Force the bulb to go dormant in late summer or early fall by withholding water and placing it in a cool, dry location for a couple of months. Resume watering and move it to a warm spot to force new growth.\nType:Bulb, Houseplant\nHeight:1 to 3 feet\nWidth:6-12 inches wide\nFlower Color:Pink, Red, White\nSeasonal Features:Spring Bloom, Winter Bloom\nSpecial Features:Low Maintenance",
                      group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-2",
                      "Arrowhead",
                      "Height:1 to 3 feet",
                      "images/arrowhead.png",
                      "",
                      "Arrowhead vine is a lush foliage plant that holds its variegation well in low light. Young plants usually remain compact mounds of foliage in various shades of green, bronze, and pink. As plants age, they develop more of a vining growth habit. Cut them back to keep them compact, or train them onto a moss pole. Arrowhead vine grows well in low to medium light with average room temperature. Keep the soil evenly moist. It is sometimes called nephthytis.\nType:Houseplant\nHeight:1 to 3 feet\nWidth:6 inches to 3 feet wide\nFoliage Color:Purple/Burgundy",
                      group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-3",
                      "Black pepper",
                      "Special Features: Good for Containers",
                      "images/blackpepper.png",
                      "",
                      "Grow your own peppercorns with this lovely houseplant. A vine that produces chains of small round fruit, black pepper thrives in full or part sun and indoor temperatures above 65 degrees F. By selecting the time of harvest, all four types of peppercorns -- black, white, green, and red -- can be harvested from the same plant. Black pepper is a slow-growing vine, and plants take three to four years to start flowering and fruiting.\nWait to water black pepper until the soil is visibly dry. When watering, thoroughly saturate the soil until a little water runs out the bottom of the pot.\nLight:Part Sun, Sun\nType:Houseplant, Vine\nHeight:1 to 3 feet\nWidth:To 3 feet wide\nSeasonal Features:Summer Bloom\nSpecial Features:Good for Containers\nZones:10-11",
                      group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-4",
                      "Dieffenbachia",
                      "Height:From 1 to 20 feet",
                      "images/dieffenbachia.png",
                      "",
                      "Dieffenbachia may be grown as a small tree with a canelike stem or as a shrubby plant with multiple stems. It thrives in low to medium light. Grow it at room temperature and keep the soil evenly moist. It is sometimes called dumb cane, a reference to the effects of its toxic sap, which can cause tongue numbness and swelling if chewed by children or pets.\nType:Houseplant\nHeight:From 1 to 20 feet\nWidth:1-3 feet wide",
                      group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-5",
                    "Croton",
                    "Height:From 1 to 8 feet",
                    "images/croton.png",
                    "",
                    "Croton is a colorful shrub with leathery leaves that are most colorful in bright light. In low light conditions new leaves will be smaller and less intensely pigmented. Grow croton at 60 to 85 degrees F with high humidity. Allow the soil surface to dry between waterings.\nType:Houseplant\nHeight:From 1 to 8 feet\nWidth:1 to 3 feet wide",
                    group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-6",
                   "Dracaena",
                   "Height:From 1 to 20 feet",
                   "images/dracaena.png",
                   "",
              "Dracaenas compose of a large group of popular foliage plants. Most grow strongly upright with long, straplike leaves variegated with white, cream, or red. Dracaenas grow well at average room temperatures but don't like cold drafts. Give plants medium to bright light to maintain best leaf color. Allow the soil to dry to the touch between waterings.\nType:Houseplant\nHeight:From 1 to 20 feet\nWidth:1-3 feet wide\nSpecial Features:Low Maintenance",
                   group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-7",
                  "English ivy",
                  "Height:Under 6 inches to 3 feet",
                  "images/ivy.png",
                  "",
                  "This versatile foliage plant grows well as a hanging basket, a groundcover beneath larger houseplants, or trained into topiary shapes. It needs medium light and grows best at temperatures between 55 and 70 degrees F. Keep the soil evenly moist and humidity high to discourage spider mites. This plant also grows as a groundcover outdoors in Zones 5-9.\nType:Houseplant, Vine\nHeight:Under 6 inches to 3 feet\nWidth:6 inches to 6 feet wide",
                  group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-8",
                  "Jade plant",
                  "Special Features:Fragrance, Low Maintenance",
                  "images/jade.png",
                  "",
                  "Jade plant is a tough, popular succulent with plump, fleshy leaves. Provide bright or intense light, and keep the soil moderately dry. Jade grows well at room temperature during the growing season, but prefers 55 degrees F during winter. (Cool winter temperatures help promote bloom.)\nType:Houseplant\nHeight:From 1 to 8 feet\nWidth:1-3 feet wide\nFlower Color:Pink\nSeasonal Features:Spring Bloom, Winter Bloom\nSpecial Features:Fragrance, Low Maintenance",
                  group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-9",
                  "Kalanchoe",
                  "Seasonal Features:Fall Bloom, Reblooming, Spring Bloom, Summer Bloom, Winter Bloom",
                  "images/kalanchoe.png",
                  "",
                  "Kalanchoes are a diverse group of succulent plants. Many are grown for their foliage, while others are primarily blooming plants. All need bright light and should be kept on the dry side except when in bloom. Room temperature is good through the growing season, but grow them cool (45-65 degrees F) during winter.\nType:Houseplant\nHeight:Under 6 inches to 8 feet\nWidth:6 inches to 3 feet wide\nFlower Color:Orange, Pink, Red, White\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Fall Bloom, Reblooming, Spring Bloom, Summer Bloom, Winter Bloom\nProblem Solvers:Drought Tolerant",
                  group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-10",
                  "Moth orchid",
                  "Special Features:Cut Flowers, Low Maintenance",
                  "images/moth.png",
                  "",
                  "One of the showiest houseplants, moth orchid is remarkably easy to grow -- despite its reputation. It also offers some of the best variety; look for miniatures that grow only a few inches tall to big hybrids that produce 3-foot-tall spikes of flowers. The blooms appear in a rainbow of colors -- from pinks to reds, yellows and oranges, lavender, green, and white. Most bloom only once a year, but the flowers can last six months or more.\nGrow moth orchid in a spot where it gets medium to bright light (such as on the windowsill of a north- or east-facing window). Water and fertilize them weekly in spring and summer when they're growing; hold off the plant food and water less in fall and winter when the plants usually rest. Like many houseplants, moth orchid does best in high humidity\nPot this plant in orchid bark or sphagnum moss; if you grow it in potting mix, the roots will likely rot and die.\nType:Houseplant\nHeight:Under 6 inches\nWidth:To 1 foot wide\nFlower Color:Blue, Green, Orange, Pink, Red, White\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Fall Bloom, Spring Bloom, Summer Bloom, Winter Bloom\nSpecial Features:Cut Flowers, Low Maintenance",
                  group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-11",
                 "Norfolk Island pine",
                 "Height:From 1 to 20 feet",
                 "images/norfolk.png",
                 "",
                 "Norfolk Island pine is a fast-growing tree that can grow to 200 feet tall in its native habitat but seldom reaches more than 10 feet tall in containers indoors. If it doesn't receive bright light indoors, its branches droop and begin to turn brown. Keep the soil moderately dry, but give it high humidity.\nType:Houseplant\nHeight:From 1 to 20 feet\nWidth:1-5 feet wide",
                 group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-12",
                "Peace lily",
                "Special Features:Low Maintenance",
                "images/peacelily.png",
                "",
               "Peace lily is an easy-care plant that tolerates low light, low humidity, and still blooms consistently. Its glossy lance-shape leaves arch gracefully from a central clump of stems. The white flowers are most common in summer, but may occur any time of year. As they age they turn green.\nType:Houseplant\nHeight:From 1 to 8 feet\nWidth:1 to 5 feet wide\nFlower Color:White\nSeasonal Features:Fall Bloom, Reblooming, Spring Bloom, Summer Bloom, Winter Bloom\nSpecial Features:Low Maintenance",
                group5));
              group5.Items.Add(new SampleDataItem("Group-5-Item-13",
               "Ponytail palm",
               "Special Features:Low Maintenance",
               "images/palm.png",
               "",
              "Despite its palm-like appearance and common name, ponytail palm is not a true palm. It is a succulent with a bulbous lower trunk that stores water. Provide it with bright light and keep it relatively dry. Normal room temperature is good for it most of the year, but in winter keep it cooler (50-55 degrees F). It is sometimes sold as Nolina recurvata.\nType:Houseplant\nHeight:From 1 to 20 feet\nWidth:1-4 feet wide\nProblem Solvers:Drought Tolerant\nSpecial Features:Low Maintenance",
              group5));
              this.AllGroups.Add(group5);

              var group6 = new SampleDataGroup("Group-6",
                      "Water Plants",
                      "",
                      "images/group6.png",
                      "Water plants are widely considered the best way to enhance water features. Sometimes called aquatic plants, there is a water plant for virtually every type of water feature, from the smallest tabletop fountains to multi-level streams and expansive ponds. The type of water plant that will best suit your landscape is largely determined by your requirements, as well as the growing characteristics of each plant.");
                      group6.Items.Add(new SampleDataItem("Group-6-Item-1",
                      "Amazon lily",
                      "Special Features:Fragrance, Good for Containers, Low Maintenance",
                      "images/amazonlily.png",
                      "",
                      "Truly a plant to wow your friends and neighbors, Amazon lily is nothing short of magnificent. In a large pond, this plant's leaves can reach 6 feet across and are covered in spiny prickles. The flowers, which appear in summer, start as thorn-covered green buds that open to large white masterpieces that fade pink. The blooms have the fragrance of ripe pineapple.\nLight:SunType:Water\nHeight:Under 6 inches\nWidth:To 20 feet wide\nFlower Color:White\nSeasonal Features:Summer Bloom\nSpecial Features:Fragrance, Good for Containers, Low Maintenance\nZones:10-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-2",
                      "Duckweed",
                      "Special Features: Good for Containers, Low Maintenance",
                      "images/duckweed.png",
                      "",
                     "Duckweed forms a fast-growing cover over pond and other water garden surfaces. Its free-floating network of disc-shape leaves spreads in a textured mat that looks like thyme foliage. The leaves, actually fronds, send out new foliage from their margins. Duckweed is a fast grower and will require thinning frequently. It is a source of nutrition for many forms of wildlife.\nLight:Part Sun, Sun\nType:Water\nHeight:Under 6 inches\nWidth:Can spread indefinitely\nSpecial Features:Good for Containers, Low Maintenance\nZones: 3-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-3",
                      "Arrowhead",
                      "Special Features: Good for Containers, Low Maintenance",
                      "images/waterarrow.png",
                      "",
                      "Arrowhead is an easy-care water plant perfect for adding a tropical feel to water gardens. This tough perennial bears attractive, arrow-shaped foliage and clusters of three-petaled white flowers. The leaves grow longer and more tapered in deeper water. Arrowhead can be an aggressive spreader. To control growth, confine this plant to containers when placed in a pond.\nLight:Sun\nType:Water\nHeight:Under 6 inches to 3 feet\nWidth:To 3 feet wide\nFlower Color:White\nSeasonal Features:Summer Bloom\nSpecial Features:Good for Containers, Low Maintenance\nZones:3-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-4",
                      "Fairy moss",
                      "Special Features:Good for Containers, Low Maintenance",
                      "images/fairy.png",
                      "",
                      "Koi love to nibble on the soft, fuzzy foliage of fairy moss. It floats freely on the surface, shading the water as it spreads to help cut down on algae growth. Its colorful fall leaf display is a great bonus; the foliage darkens to purple-red at season's end. Bring some clumps indoors to overwinter fairy moss in an aquarium or pan of water to replenish the pond supply in the next season. Otherwise, they will freeze and then thaw in the spring.\nLight:Part Sun, Shade, Sun\nType:Water\nHeight:Under 6 inches\nWidth:Can spread indefinitely\nSpecial Features:Good for Containers, Low Maintenance\nZones: 6-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-5",
                      "Lotus",
                      "Special Features:Cut Flowers, Fragrance, Good for Containers",
                      "images/lotus.png",
                      "",
                      "The sacred flower of Asian countries launches huge plate-sized leaves from underwater stems and then unfurls spectacular, multipetaled flowers. As the blooms open, they reveal a crown of golden stamens that gradually gives way to a large ornamental seedhead. Lotus plants are quite hardy and can safely spend the winter dormant in a pond. As the weather warms in late spring, the plants will resprout and spin out new foliage. Plant lotus in pots and sink them to a depth of 2 to 6 inches below the water surface for the best display.\nLight:Sun\nType:Water\nHeight:3 to 8 feet\nWidth:2-6 feet wide\nFlower Color:Pink, White\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Summer Bloom\nSpecial Features:Cut Flowers, Fragrance, Good for Containers\nZones: 5-9",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-6",
                      "Marsh marigold",
                      "Height:1 to 3 feet",
                      "images/marsh.png",
                      "",
                      "This flower likes wet conditions so much that it's often recommended for bog and water gardens where it lights things up with bright yellow flowers. A native of wetlands, marsh marigold forms foot-tall mounds of foliage topped with 1- to 2-inch-wide yellow blooms (a white form is also available) in early spring. It's also a good selection for chronically soggy or poorly drained sites. It often goes dormant after it blooms.\nLight:Part Sun, Sun\nType:Perennial, Water\nHeight:1 to 3 feet\nWidth:12-15 inches wide\nFlower Color:\nWhite\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Spring Bloom\nZones:2-7",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-7",
                      "Papyrus",
                      "Special Features:Good for Containers, Low Maintenance",
                      "images/papyrus.png",
                      "",
                      "Papyrus sends out sprays of leaves that jut out from the stems like a fireworks display. The leafy clusters may form plantlets that you can separate and grown individually. Grow papyrus in a weighted pot so that the stems rise above the water surface in a pond, or grow it in moist soil at water's edge.\nLight:Part Sun, Sun\nType:Water\nHeight:From 1 to 8 feet\nWidth:2-4 feet wide\nSpecial Features:Good for Containers, Low Maintenance\nZones:9-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-8",
                      "Parrot's feather",
                      "Special Features:Good for Containers",
                      "images/parrotfeather.png",
                      "",
                      "Parrot's feather is a versatile plant that you can grow underwater (to add oxygen, give fish a place to hide, and cut down on algae in ponds), floating above the water, or in wet soil at water's edge. It earned its moniker from its dense plumes of fine-texture foliage. The feathery branches grow at the tops of long, floating stems.\nParrot's feather is considered an invasive plant in the waterways of temperate states, where its growth can quickly become out of control.\nLight:Part Sun, Sun\nType:Water\nHeight:Under 6 inches\nWidth:Can spread indefinitely\nSpecial Features:Good for Containers\nZones:6-11",
                      group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-9",
                    "Pitcher plant",
                    "Special Features:Cut Flowers, Good for Containers",
                    "images/pitcher.png",
                    "",
                   "Pitcher plants are one of those cool carnivorous plants; they can devour insects. But don't let this amazing fact overshadow their inherent beauty. They produce fascinating pendant chartreuse or purple flowers in spring. Pitcher plants are fascinating to grow, and adapt well to containers where the plants can be observed up close. In mild regions, they can also be grown in acid bog gardens. They do not need a diet of insects -- the insects are attracted by nectar at the base of the pitchers and slide down and drown in collected liquid at the base. The tall pitchers of some species are cut and dried for indoor arrangements, but only remove a few to retain the vitality of the plants.\nLight:Part Sun, Sun\nType:Perennial, Water\nHeight:Under 6 inches to 8 feet\nWidth:3 feet wide\nFlower Color:Green, Pink, Red\nSeasonal Features:Spring Bloom\nProblem Solvers:Deer Resistant\nSpecial Features:Cut Flowers, Good for Containers\nZones:2-10",
                    group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-10",
                   "Society garlic",
                   "Special Features:Attracts Birds, Fragrance, Low Maintenance",
                   "images/saciety.png",
                   "",
                  "The leaves look like chives and if you walk by a planting of this South African native bulb and brush the foliage, you'll catch a whiff of garlic. The beautiful clusters of lavender-pink flowers have a sweet fragrance, similar to hyacinth perfume. They open up on tall stems from early summer until late fall. Noted for its drought tolerance, society garlic has become a staple in southern California landscapes.\nLight:Sun\nType:Bulb, Water\nHeight:Under 6 inches to 3 feet\nWidth:To 2 feet wide\nFlower Color:Blue, Pink\nSeasonal Features:Spring Bloom, Summer Bloom\nProblem Solvers:Deer Resistant, Drought Tolerant\nSpecial Features:Attracts Birds, Fragrance, Low Maintenance\nZones:7-9",
                   group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-11",
                   "Thalia",
                   "Special Features:Good for Containers, Low Maintenance",
                   "images/thalia.png",
                   "",
                   "Colorful and tall, this cousin of the canna develops huge leaves that turn gold in the fall. Its flowers are perched in bunches atop tall stems and add wonderful texture to water gardens. The plant dominates the pond with its far-reaching foliage, so is best limited to one per small pond. Plant this tall aquatic in a large pot to stabilize and protect it from the wind.\nLight:\nPart Sun, Sun\nType:Water\nHeight:Under 6 inches to 20 feet\nWidth:To 6 feet wide\nFlower Color:Blue\nSeasonal Features:Summer Bloom\nSpecial Features:Good for Containers, Low Maintenance\nZones: 7-11",
                   group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-12",
                   "Water lettuce",
                   "Special Features:Good for Containers, Low Maintenance",
                   "images/lettuce.png",
                   "",
                   "This water plant is grown for its beautiful, velvety foliage that really does resemble a dense carpet of lettuce heads flowering on the water. It can be an important plant for ponds as it shades the water and gives small fish a place to hide. In cold climates, treat this tender floating plant as an annual and replace every year.\nNote: In warm-winter climates, water lettuce can be invasive. Check to see if the plant is banned in your area before planting it.\nLight:Part Sun, Sun\nType:Water\nHeight:Under 6 inches to 3 feet\nWidth:Can spread indefinitely\nSpecial Features:Good for Containers, Low Maintenance\nZones:10-11",
                   group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-13",
                 "Water hyacinth",
                 "Special Features: Good for Containers, Low Maintenance",
                 "images/hyacinth.png",
                 "",
                 "A jewel in the water garden but invasive in waterways, water hyacinth is a pond fish's best friend, providing shelter and feeding area. Rosettes of glossy green leaves float leisurely across a pond, gradually covering the surface and sending down thick roots that shelter fish. In warm weather, the plants send up lavender bloom spikes that last about a day. It should be planted early in the season so its spread outpaces algae, and thin out old plants every year to reinvigorate growth. Because it's a tender plant, it requires overwintering indoors in an aquarium to survive from year to year.\nNote: This plant can be invasive in warm-weather areas. Check local restrictions before planting it.\nLight:Part Sun, Sun\nType:Water\nHeight:Under 6 inches\nWidth:Can spread indefinitely\nFlower Color:Blue\nSeasonal Features:Summer Bloom\nSpecial Features:Good for Containers, Low Maintenance\nZones:9-11",
                 group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-14",
                 "Water lily",
                 "Special Features:Fragrance, Good for Containers, Low ",
                 "images/waterlily.png",
                 "",
                "Water lilies offer a floating mat of foliage crowned with resplendent blooms that open every morning, then close for the afternoon (though night-blooming water lilies open at night and close every morning). Each bloom generally lasts 3-4 days, and then is quickly replaced. In addition to the luminous color choices, water lilies are often fragrant. Some dwarf varieties are available for container water gardeners. To keep lilies vigorous, divide them every 1-4 years.\nLight:Sun\nType:Water\nHeight:Under 6 inches\nWidth:3-12 feet wide\nFlower Color:Blue, Orange, Pink, Red, White\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Fall Bloom, Summer Bloom\nSpecial Features:Fragrance, Good for Containers, Low Maintenance\nZones:3-11",
                 group6));
              group6.Items.Add(new SampleDataItem("Group-6-Item-15",
                 "Water snowflake",
                 "Special Features:Good for Containers, Low Maintenance",
                 "images/snowflake.png",
                 "",
                 "Nympohides is a charming plant to float in the water garden, both for its fringed, snowflake-like flowers and bunches of glossy, heart-shape leaves. The white form is tender, but in cold climates, you can overwinter it by bringing pots indoors or by submerging them in a container of water in a cool place.\nLight:Part Sun, Sun\nType:Water\nHeight:Under 6 inches\nWidth:1-3 feet wide\nFlower Color:White\nFoliage Color:Chartreuse/Gold\nSeasonal Features:Summer Bloom\nSpecial Features:Good for Containers, Low Maintenance\nZones: 5-11",
                 group6));
              this.AllGroups.Add(group6);
            
          }
      }
  }
