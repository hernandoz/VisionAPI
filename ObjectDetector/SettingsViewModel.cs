﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Xamarin.Forms;

namespace ObjectDetector
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
            OpenCustomVisionCommand = new Command(async () => await Xamarin.Essentials.Launcher.OpenAsync("https://customvision.ai"));
            OkCommand = new Command(async () => await Ok(), () => !string.IsNullOrWhiteSpace(PredictionKey) && !string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out var g));
            ConfigureCanCancelCommand = new Command(async () => CanCancel = !string.IsNullOrWhiteSpace(await KeyService.GetPredictionKey()) && !string.IsNullOrWhiteSpace(await KeyService.GetProjectId()));
            CancelCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopModalAsync());
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

            IsEnabled = false;
        }

        public async Task SetUp()
        {
            try
            {
                PredictionKey = await KeyService.GetPredictionKey();
                ProjectId = await KeyService.GetProjectId();
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task Ok()
        {
            IsEnabled = false;
            try
            {
                var endpoint = new PredictionEndpoint
                {
                    ApiKey = KeyService.PK
                };

                if (Guid.TryParse(KeyService.PI, out var pId))
                {
                    var imagePath = "ObjectDetector.Images.single.png";
                    var assembly = typeof(SettingsViewModel).GetTypeInfo().Assembly;

                    using (var stream = assembly.GetManifestResourceStream(imagePath))
                    {
                        await endpoint.PredictImageWithNoStoreAsync(pId, stream);
                    }

                    await KeyService.SetPredictionKey(KeyService.PK);
                    await KeyService.SetProjectId(KeyService.PI);

                    Analytics.TrackEvent("Updating keys");

                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    Analytics.TrackEvent("Failed updating keys", new Dictionary<string, string> { { "Error", "The project Id is not a valid GUID" } });
                    await Application.Current.MainPage.DisplayAlert("Error", "The project Id is not a valid GUID", "OK");
                }
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Failed updating keys", new Dictionary<string, string> { { "Error", "The project Id and prediction key don't match an existing project" } });
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Action", "Testing key and project id" } });
                await Application.Current.MainPage.DisplayAlert("Error", "The project Id and prediction key don't match an existing project", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        public ICommand OpenCustomVisionCommand { get; }
        public ICommand ConfigureCanCancelCommand { get; }
        public Command  OkCommand { get; }
        public ICommand CancelCommand { get; }

        string predictionKey = KeyService.PK;
        public string PredictionKey
        {
            get => predictionKey;
            set
            {
                Set(ref predictionKey, value);
                //OkCommand.ChangeCanExecute();
            }
        }

        string projectId = KeyService.PI;
        public string ProjectId
        {
            get => projectId;
            set
            {
                Set(ref projectId, value);
                //OkCommand.ChangeCanExecute();
            }
        }

        bool canCancel;
        public bool CanCancel
        {
            get => canCancel;
            set => Set(ref canCancel, value);
        }
    }
}

