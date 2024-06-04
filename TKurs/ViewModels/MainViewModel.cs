using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using SciChart.Charting3D;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.Model.ChartSeries;
using SciChart.Charting3D.RenderableSeries;
using SciChart.Charting3D.Visuals.RenderableSeries;
using SciChart.Data.Model;
using SkiaSharp;
using TKurs.Context;
using TKurs.Model;
using TKurs.Model.Calculators;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using Task = TKurs.Model.Task;

namespace TKurs.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private DoubleRange t1Range;
        [ObservableProperty] private DoubleRange t2Range;

        public ObservableCollection<IRenderableSeries3DViewModel> RenderableSeriesViewModels { get; set; }


        public UniformGridDataSeries3D<double> Series { get; set; }
        [ObservableProperty] private AxisBase3DViewModel xAxis;
        [ObservableProperty] private AxisBase3DViewModel yAxis;
        [ObservableProperty] private AxisBase3DViewModel zAxis;

        [ObservableProperty] private double resultT1;
        [ObservableProperty] private double resultT2;
        [ObservableProperty] private double optimalCost;


        [ObservableProperty] private double minT1;
        [ObservableProperty] private double minT2;
        [ObservableProperty] private double maxT1;
        [ObservableProperty] private double maxT2;
        [ObservableProperty] private double tempRe;
        [ObservableProperty] private List<TableData> tableDatas;

        [ObservableProperty] private double step;

        [ObservableProperty] private FiltrationProcess _filtrationProcess;

        [ObservableProperty] private bool findMInimum;

        [ObservableProperty] private Visibility _visibilityError;
        [ObservableProperty] private Visibility _visibilityT;

        private readonly OptimDbContext _context;

        [ObservableProperty] private ObservableCollection<User> users;

        [ObservableProperty] private ObservableCollection<Task> tasks;

        [ObservableProperty] private ObservableCollection<Method> methods;

        [ObservableProperty] private Method method;
        [ObservableProperty] private Task task;
        [ObservableProperty] private Task selectedTask;

        [ObservableProperty] private Method selectedMethod;


        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);


            if (e.PropertyName == nameof(SelectedTask))
            {
                if (SelectedTask == null)
                {
                    return;
                }
                else if (SelectedTask.Real)
                {
                    VisibilityError = Visibility.Collapsed;
                    VisibilityT = Visibility.Visible;
                }
                else
                {
                    VisibilityError = Visibility.Visible;
                    VisibilityT = Visibility.Collapsed;
                }
            }
        }

        public MainViewModel()
        {
            RenderableSeriesViewModels = new ObservableCollection<IRenderableSeries3DViewModel>();


            MinT1 = -5;
            MinT2 = -1;
            MaxT1 = 0;
            MaxT2 = 5;
            step = 0.1;
            TempRe = 1.5;


            YAxis = new NumericAxis3DViewModel()
            {
                StyleKey = "CustomNumeric3DStyle",
                AxisTitle = "T1",
            };

            XAxis = new NumericAxis3DViewModel
            {
                StyleKey = "CustomNumeric3DStyle",

                AxisTitle = "T2"
            };

            ZAxis = new NumericAxis3DViewModel
            {
                StyleKey = "CustomNumeric3DStyle",
                AxisTitle = "Cost"
            };


            VisibilityT = Visibility.Collapsed;
            VisibilityError = Visibility.Collapsed;
            FiltrationProcess = new FiltrationProcess();
            _context = new OptimDbContext();
            Users = new ObservableCollection<User>(_context.Users.ToList());
            Users.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    _context.Users.AddRange(e.NewItems.Cast<User>());
                else if (e.OldItems != null)
                    _context.Users.RemoveRange(e.OldItems.Cast<User>());
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                    foreach (User item in e!.NewItems!)
                        _context.Users.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            };
            Tasks = new ObservableCollection<Task>(new List<Task>()
            {
                new Task()
                {
                    Real = true,
                    Formulaa = "V =  α* G* (Т1^2 +β Т2 – µ \u2206р1  )^N +  γ (β1Т1 + Т2^2 – µ1\u2206р2 )^N",
                    Descriptiom =
                        "Объектом оптимизации  является процесс фильтрования с использованием  установки, имеющей две фильтрационные перегородки, на каждой из которых поддерживается свой температурный режим. Известно, что объемный расход фильтрата V(м^3/ч)  связан с температурами T1 и T2 на каждой перегородке следующим образом:\r\nV =  α* G* (Т1^2 +β Т2 – µ \u2206р1  )^N +  γ (β1Т1 + Т2^2 – µ1\u2206р2 )^N\r\n"
                }
            });
            var tasks = _context.Tasks.ToList();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            Tasks.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    _context.Tasks.AddRange(e.NewItems.Cast<Task>());
                else if (e.OldItems != null)
                    _context.Tasks.RemoveRange(e.OldItems.Cast<Task>());
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                    foreach (Task item in e!.NewItems!)
                        _context.Tasks.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            };

            Methods = new ObservableCollection<Method>(new List<Method>()
            {
                new Method()
                {
                    Name = "Комплексный метод бокса",
                    Real = true,
                    Calculator = new BoxMethodOptimizer()
                }
            });
            var methods = _context.Mehods.ToList();
            foreach (var method in methods)
            {
                Methods.Add(method);
            }

            Methods.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    _context.Mehods.AddRange(e.NewItems.Cast<Method>());
                else if (e.OldItems != null)
                    _context.Mehods.RemoveRange(e.OldItems.Cast<Method>());
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                    foreach (Method item in e!.NewItems!)
                        _context.Mehods.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            };
            SelectedTask = null;
            SelectedMethod = Methods[0];
        }


        public User User { get; set; }


        [RelayCommand]
        public void DeleteUser(User user)
        {
            Users.Remove(user);
        }


        [RelayCommand]
        public void SaveUsers()
        {
            foreach (var item in Users)
            {
                _context.Users.Update(item);
                _context.SaveChanges();
            }
           
        }

        [RelayCommand]
        public void EditMethod(Method method)
        {
            Method = method;
        }

        [RelayCommand]
        public void Calculate()
        {
            if (SelectedMethod == null)
                return;
            if (SelectedMethod.Real)
            {
                var random = new Random(0);

                if (RenderableSeriesViewModels.Count != 0)
                    RenderableSeriesViewModels.Remove(RenderableSeriesViewModels.Last());
                List<double> xS = new List<double>();
                List<double> yS = new List<double>();
                List<double> zS = new List<double>();


                var result = SelectedMethod.Calculator.DataOptimize(FiltrationProcess, MinT1, MaxT1, MinT2, MaxT2,
                    tempRe, Step,
                    FindMInimum);
                foreach (var item in result.Item4)
                {
                    xS.Add(Math.Round(item.Cost,1));
                    yS.Add(Math.Round(item.T1,1));
                    zS.Add(Math.Round(item.T2,1));
                }

                TableDatas = new List<TableData>();
                for (int i = 0; i < xS.Count; i++)
                {
                   TableDatas.Add(new TableData(yS[i], zS[i], xS[i]));
                }
                

                var T1size = (int)((Math.Abs(MinT1) + Math.Abs(MaxT1)) / Step) + 2;
                var T2size = (int)((Math.Abs(MinT2) + Math.Abs(MaxT2)) / Step) + 2;

                T1Range = new DoubleRange(MinT1, MaxT1);
                T2Range = new DoubleRange(MinT2, MaxT2);
               Series= new UniformGridDataSeries3D<double>(T1size, T2size);
               Series.IsDirty = false;
               Series.IsMeshDirty = false;
               Series.IsHeightmapDirty = false;
               Series.StartX = MinT1;
               Series.StepX = Step; ;
               Series.StartZ = MinT2;
               Series.StepZ = Step;
              
               var zi = 0;
               

                for (double z =MinT2; z < MaxT2; z+= Step)
                {
                    var yi = 0;
                    zi++;
                    for (double y = MinT1; y < MaxT1; y+= Step)
                    {
                        if (0.5 * y + z <= tempRe)
                        {
                            Series[zi, yi] = FiltrationProcess.CalculateCost(FiltrationProcess.CalculateVolume(y, z));
                            
                        }
                        yi++;
                    }
                }

               

                
                ResultT1 = Math.Round(result.T1, 3);
                ResultT2 = Math.Round(result.T2, 3);
                OptimalCost = Math.Round(result.OptimalCost, 3);

                HeatMapValue = new ObservableCollection<WeightedPoint>();
                



                //var qwe = zS.Max();

                RenderableSeriesViewModels.Add(new SurfaceMeshRenderableSeries3DViewModel()
                {
                    StrokeThickness = 1,
                    
                    DataSeries = Series,
                    DrawSkirt = false,
                    Opacity = 0.9,
                    DrawMeshAs = DrawMeshAs.SolidWireFrame,
                    MeshPaletteMode = MeshPaletteMode.HeightMapInterpolated
                });
                SearchedPoint = new ObservableCollection<ISeries>
                {
                   
                    
                    new HeatSeries<WeightedPoint>
                    {
                       PointPadding = new LiveChartsCore.Drawing.Padding(0),
                       

                           HeatMap = new[]
                     {

                            SKColors.Black.AsLvcColor(),
                            SKColors.Gray.AsLvcColor(),
                            SKColors.Blue.AsLvcColor(),
                            SKColors.Yellow.AsLvcColor(),
                            SKColors.Orange.AsLvcColor(),
                            SKColors.Red.AsLvcColor(),
                            SKColors.Red.AsLvcColor(),

                         },
                           Values = HeatMapValue


                    },
                    new ScatterSeries<ObservablePoint>
                    {
                        ZIndex = 1500,

                        Values = new ObservableCollection<ObservablePoint>
                        {
                           new(ResultT1,ResultT2)
                        }
                    }


                };

                for (int i = 0;i < xS.Count; i++)
                {
                    HeatMapValue.Add(new(yS[i], zS[i], xS[i]));
                }

                XAxesV = new Axis[]
                {
                     new Axis
                       {
                            MinLimit = MinT1,
                            MaxLimit = MaxT1,
                            Name = "T1 \u00b0C",
                       }
                };

                YAxesV = new Axis[]
                {
                     new Axis
                       {
                            MinLimit = MinT2,
                            MaxLimit = MaxT2,
                            Name = "T2 \u00b0C",
                       }
                };

            }
        }

        public ObservableCollection<WeightedPoint> HeatMapValue { get; set; }
       

        [ObservableProperty]
        private IEnumerable<ISeries> searchedPoint;

        [ObservableProperty]
        private Axis[] xAxesV =
        {
              new Axis
                       {

                            Name = "T1 \u00b0C",
                       }

        };
        [ObservableProperty]
        private Axis[] yAxesV =
        {
            new Axis
            {
                Name = "T2 \u00b0C",

            }
        };
        [RelayCommand]
        public void Save()
        {
            if (Method == null)
                return;
            _context.Mehods.Entry(Method).State = EntityState.Modified;
            _context.SaveChanges();
        }

        [RelayCommand]
        public void AddTask()
        {
            Tasks.Add(new Task() { Formulaa = "Empty", Real = false, Descriptiom = "Empty" });
        }

        [RelayCommand]
        public void DeleteTask(Task task)
        {
            Tasks.Remove(task);
        }

        [RelayCommand]
        public void EditTask(Task task)
        {
            Task = task;
        }

        [RelayCommand]
        public void SaveTask()
        {
            if (Task == null)
                return;
            _context.Tasks.Entry(Task).State = EntityState.Modified;
            _context.SaveChanges();
        }


        [RelayCommand]
        public void AddMethod()
        {
            Methods.Add(new Method() { Name = "Empty", Real = false });
        }

        [RelayCommand]
        public void DeleteMethod(Method method)
        {
            Methods.Remove(method);
        }
    }
}