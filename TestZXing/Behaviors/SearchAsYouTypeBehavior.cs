using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestZXing.Behaviors
{
    public class SearchAsYouTypeBehavior : BehaviorBase<SearchBar>
    {
        //private readonly IObservable<System.Reactive.EventPattern<Xamarin.Forms.TextChangedEventArgs>> _subscription;

        public const int DefaultMinimumSearchIntervalMiliseconds = 300;

        private CancellationTokenSource _cancellationTokenSource;

        //public SearchAsYouTypeBehavior()
        //{
        //    _subscription = Observable.FromEventPattern<TextChangedEventArgs>(
        //    handler => AssociatedObject.TextChanged += handler,
        //    handler => AssociatedObject.TextChanged -= handler);
        //}

        public static readonly BindableProperty SearchCommandProperty =
            BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchAsYouTypeBehavior),
                propertyChanged: SearchCommandChanged);

        public static readonly BindableProperty MinimumSearchIntervalMilisecondsProperty =
            BindableProperty.Create(nameof(MinimumSearchIntervalMiliseconds), typeof(int),
                typeof(SearchAsYouTypeBehavior), DefaultMinimumSearchIntervalMiliseconds);

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public int MinimumSearchIntervalMiliseconds
        {
            get => (int)GetValue(MinimumSearchIntervalMilisecondsProperty);
            set => SetValue(MinimumSearchIntervalMilisecondsProperty, value);
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            base.OnDetachingFrom(bindable);
            AssociatedObject.TextChanged -= Search;
        }

        private static void SearchCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (SearchAsYouTypeBehavior)bindable;
            behavior.SearchCommandChanged(newValue);
        }

        private void SearchCommandChanged(object newCommand)
        {
            if (newCommand is ICommand)
                AssociatedObject.TextChanged += Search;
            else
                AssociatedObject.TextChanged -= Search;
        }

        private async void Search(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            //_subscription.Throttle(TimeSpan.FromMilliseconds(MinimumSearchIntervalMiliseconds));

            //_subscription.ObserveOn(SynchronizationContext.Current);

            //_subscription.Select(eventPattern => AssociatedObject.Text)
            // .DistinctUntilChanged();
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                await Task.Delay(MinimumSearchIntervalMiliseconds, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    return;

                ExecuteSearch();
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
}

        private void ExecuteSearch()
        {
            //_subscription.Subscribe(query => Device.BeginInvokeOnMainThread(() =>
            //{
            //    SearchCommand?.Execute(query);
            //})
            //);
            Device.BeginInvokeOnMainThread(() =>
            {
                SearchCommand?.Execute(null);

                if (!AssociatedObject.IsFocused)
                    AssociatedObject.Focus();
            });
        }


    }
}
