
using BirdsCommon;
using BirdsCommon.Repository;
using System.Collections.ObjectModel;

namespace BirdsViewModels
{
    public class MainBirdsViewModel : ViewModelBase, INavigationService
    {
        private readonly IRepository<Bird> birdsRepository;
        private readonly BirdsViewModel birdsVM;
        private readonly AddBirdViewModel addBirdVM;

        public MainBirdsViewModel(IRepository<Bird> birdsRepository)
        {
            this.birdsRepository = birdsRepository;
            Current = birdsVM = new(birdsRepository);
            addBirdVM = new(birdsRepository);
            //privateBirds = birdsRepository.GetObservableCollection();
            //Birds = new(privateBirds);
            Birds = birdsRepository.GetObservableCollection();
        }

        public async Task LoadAsync() => await birdsRepository.LoadAsync();


        //private readonly ObservableCollection<Bird> privateBirds;
        public ReadOnlyObservableCollection<Bird> Birds { get; }

        private static readonly ReadOnlyCollection<string> privateBirdNameGroups
            = Array.AsReadOnly(["Nuthatch", "Great tit", "Black-capped chickadee", "Sparrow", "Amadina", "All of them", "Only inactive", "Only active"]);

        /// <summary>������������� ����������� ��������� <see cref="privateBirdNameGroups"/>. 
        /// ����� ���� �������� ����������� �����, �� ��� ���������� �������� ������� ��� ������ ��������.</summary>
        public ReadOnlyCollection<string> BirdNameGroups => privateBirdNameGroups;

        public DateOnly Departure { get; } = DateOnly.FromDateTime(DateTime.Now);
        public RelayCommand DeleteBirdCommand => GetCommand<Bird>
        (
            async bird =>
            {
                // ����� ����� ��������� ���������� ������?

                // �������� �����, ����� �������� ��������� ������������ ������ ����� ���������� � �����������.
                Bird bird1 = bird.Clone();

                // �������� ��������� � ����.
                bird1.Departure = Departure;
                bird1.IsActive = false;

                // ���������� � �����������.
                // ����������� �������� ������������� ������� ������� ����� ��������� ����������.
                Bird bird2 = await birdsRepository.UpdateAsync(bird1);
            },
            bird => bird.IsActive
        );

        #region [ Properties ]
        public object? Current { get => Get<object>(); private set => Set(value); }
        #endregion

        #region [ Methods ]
        public void NavigateTo(object? viewModel)
        {
            Current = viewModel;
        }
        #endregion


        public RelayCommand NavigateToType => GetCommand<Type>(t =>
        {
            object? curr;
            if (t == typeof(BirdsViewModel))
            {
                curr = birdsVM;
            }
            else if (t == typeof(AddBirdViewModel))
            {
                curr = addBirdVM;
            }
            else
            {
                curr = null;
            }
            NavigateTo(curr);
        });

        public RelayCommand AddBirdCommand => GetCommand<Bird>
        (
            async bird =>
            {
                await birdsRepository.AddAsync(bird);
            },
            bird => !string.IsNullOrWhiteSpace(bird.Name)
        );

    }
}
