using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Sahaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Make a place to store the timer
        private readonly DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();
            //Configure the timer
            _timer = new DispatcherTimer
            {
                //Set the interval between ticks (in this case 2 seconds to see it working)
                Interval = TimeSpan.FromSeconds(2)
            };

            //Change what's displayed when the timer ticks
            _timer.Tick += ChangeImage;
            _timer.Tick += ChangeImage1;
            //Start the timer
            _timer.Start();
        }
        private void ChangeImage(object sender, object o)
        {
            //Get the number of items in the flip view
            var totalItems = TheFlipView.Items.Count;
            //Figure out the new item's index (the current index plus one, if the next item would be out of range, go back to zero)
            var newItemIndex = (TheFlipView.SelectedIndex + 1) % totalItems;
            //Set the displayed item's index on the flip view
            TheFlipView.SelectedIndex = newItemIndex;
            //Get the number of items in the flip view

        }
        private void ChangeImage1(object sender, object o)
        {
            var totalItems = TheFlipView1.Items.Count;
            //Figure out the new item's index (the current index plus one, if the next item would be out of range, go back to zero)
            var newItemIndex = (TheFlipView1.SelectedIndex + 1) % totalItems;
            //Set the displayed item's index on the flip view
            TheFlipView1.SelectedIndex = newItemIndex;
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void green_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GroupedItemsPage1));
        }

        private void problems_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemDetailPage2));
        }

       
    }
}
