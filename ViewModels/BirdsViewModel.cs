﻿using BirdsWPF.Core;
using BirdsWPF.Models;
using BirdsWPF.Repositories.Abstract;
using System.Collections.ObjectModel;

namespace BirdsWPF.ViewModels
{
    public class BirdsViewModel : ObservableObject
    {
        #region [ Fields ]
        private readonly IBirdRepository _birdRepository;
        private int _numOfBirds = 0;
        private RelayCommand<BirdEntity>? _deleteBirdCommand;
        #endregion

        public BirdsViewModel(IBirdRepository birdRepository)
        {
            _birdRepository = birdRepository;
            LoadBirds();
        }

        #region [ Properties ]
        public ObservableCollection<BirdEntity> Birds { get; } = [];
        public string? Name { get; set; }
        public List<string> BirdsNames => ["Nuthatch", "Great tit", "Black-capped chickadee", "Sparrow", "Amadina", "All of them", "Only inactive", "Only active"];
        public DateOnly Departure { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string NumberOfBirds { get => "Number of birds: " + _numOfBirds.ToString(); set => Set(ref _numOfBirds, int.Parse(value)); }
        #endregion

        #region [ Commands ]
        public RelayCommand<BirdEntity> DeleteBirdCommand => _deleteBirdCommand ??= new RelayCommand<BirdEntity>
        (
            async bird =>
            {
                bird.Departure = Departure;
                bird.IsActive = false;
                DeleteBirdCommand.RaiseCanExecuteChanged(); // Обновление команды
                await _birdRepository.UpdateAsync(bird);
            },
            bird => bird.IsActive
        );
        #endregion

        #region [ Methods ]
        private async void LoadBirds()
        {
            var birds = await _birdRepository.GetAllAsync();
            Birds.Clear();
            foreach (var bird in birds)
                Birds.Add(bird);

            #region [ Здесь будет выборка по свойству Name, которое будет браться из ComboBox ]
            //switch (Name)
            //{
            //    case "Только неактивные":
            //        foreach (var bird in birds.Where(b => b.IsAlive == false))
            //            Birds.Add(bird);
            //        break;
            //    case "Только активные":
            //        foreach (var bird in birds.Where(b => b.IsAlive == true))
            //            Birds.Add(bird);
            //        break;
            //    case "Поползень":
            //    case "Большак":
            //    case "Гайка":
            //    case "Воробей":
            //    case "Амадин":
            //        foreach (var bird in birds.Where(b => b.Name == Name))
            //            Birds.Add(bird);
            //        break;
            //    default:
            //        foreach (var bird in birds)
            //            Birds.Add(bird);
            //        break;
            //}
            #endregion

            NumberOfBirds = Birds.Count.ToString();
        }

        #region [ Тут будет команда по показу сущностей в зависимости от выбранного из ComboBox ]
        //private void ShowOnly()
        //{
        //    LoadBirds();
        //}
        #endregion

        #endregion
    }
}