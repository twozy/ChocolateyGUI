﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChocolateyGui.Services;
////using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels
{
    public sealed class SourcesViewModel : Conductor<ISourceViewModelBase>.Collection.OneActive
    {
        private bool _firstLoad = true;

        public SourcesViewModel(ISourceService sourceService,
            Func<string, LocalSourceViewModel> localSourceVmFactory,
            Func<Uri, string, RemoteSourceViewModel> remoteSourceVmFactory)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException(nameof(sourceService));
            }

            if (localSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(localSourceVmFactory));
            }

            if (remoteSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(remoteSourceVmFactory));
            }

            Items.Add(localSourceVmFactory("This PC"));

            foreach (var source in sourceService.GetSources())
            {
                Items.Add(remoteSourceVmFactory(new Uri(source.Url), source.Name));
            }
        }

        protected override void OnViewReady(object view)
        {
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Where(p => p.EventArgs.PropertyName == nameof(ActiveItem))
                .Subscribe(p => DisplayName = $"Source - {ActiveItem.DisplayName}");

            if (_firstLoad)
            {
                ActivateItem(Items[0]);
                _firstLoad = false;
            }
        }
    }
}