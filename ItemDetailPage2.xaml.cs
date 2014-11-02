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
using Windows.ApplicationModel.DataTransfer;
using System.Text;
using Windows.Storage.Streams;
using Sahaya.data;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Sahaya
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage2 : Sahaya.Common.LayoutAwarePage
    {
        private readonly DispatcherTimer _timer;

     
        public ItemDetailPage2()
        {
            this.InitializeComponent();
            //Configure the timer
            _timer = new DispatcherTimer
            {
                //Set the interval between ticks (in this case 2 seconds to see it working)
                Interval = TimeSpan.FromSeconds(6)
            };

            //Change what's displayed when the timer ticks
            _timer.Tick += ChangeImage;
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
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
     

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
        }
        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
             request.Data.Properties.Title = "Problems";
            request.Data.Properties.Description = "In City about Green and Human";

            // Share recipe text
            var recipe = "\r\nSUBJECT\r\n";
            recipe += String.Join("\r\n", t1.Text);
            recipe += ("\r\n\r\nMESSAGE\r\n" + t2.Text);
            request.Data.SetText(recipe);
        }
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            // TODO: Derive a serializable navigation parameter and assign it to pageState["SelectedItem"]
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
