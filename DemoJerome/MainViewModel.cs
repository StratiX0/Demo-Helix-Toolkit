
namespace DemoJerome
{
    using System;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;

    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Color = System.Windows.Media.Color;
    using Plane = SharpDX.Plane;
    using Vector3 = SharpDX.Vector3;
    using Colors = System.Windows.Media.Colors;
    using Color4 = SharpDX.Color4;
    using System.Windows.Media;
    using System.Windows.Documents;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class MainViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public string Name { get; set; } // Nom de la fenetre
        public MainViewModel ViewModel { get { return this; } } // Recuperer le ViewModel
        public MeshGeometry3D Model { get; private set; } // Modele 3D
        public MeshGeometry3D Floor { get; private set; } // Modele 3D du sol

        public MeshGeometry3D LightSphere { get; private set; } // Modele 3D de sphere lumiere
        public MeshGeometry3D Sphere { get; private set; } // Modele 3D de sphere
        public MeshGeometry3D Sphere2 { get; private set; } // Modele 3D de sphere
        public Transform3D ModelTransform { get; private set; } // Transformation du modele 3D
        public Transform3D FloorTransform { get; private set; } // Transformation du modele 3D du sol
        public Transform3D Light1Transform { get; private set; } // Transformation de la lumiere
        public Transform3D Light2Transform { get; private set; } // Transformation de la lumiere
        public Transform3D Light3Transform { get; private set; } // Transformation de la lumiere
        public Transform3D Light1DirectionTransform { get; private set; } // Transformation de la direction de la lumiere
        public Transform3D Light2DirectionTransform { get; private set; } // Transformation de la direction de la lumiere
        public Transform3D Light3DirectionTransform { get; private set; } // Transformation de la direction de la lumiere
        public Transform3D Object1Transform { get; private set; } // Transformation de l'objet 3D
        public PhongMaterial ModelMaterial { get; set; } // Materiau du modele 3D
        public PhongMaterial ReflectMaterial { get; set; } // Materiau reflechissant
        public PhongMaterial FloorMaterial { get; set; } // Materiau du sol
        public PhongMaterial LightModelMaterial { get; set; } // Materiau de la lumiere
        public PhongMaterial LightModelMaterial2 { get; set; } // Materiau de la lumiere
        public PhongMaterial ObjectMaterial { set; get; } = PhongMaterials.Blue; // Materiau de l'objet 3D

        //public Vector3D Light1Direction { get; set; } // Direction de la lumiere
        public Color Light1Color { get; set; } // Couleur de la lumiere
        public Color Light2Color { get; set; } // Couleur de la lumiere
        public Color Light3Color { get; set; } // Couleur de la lumiere
        public Color AmbientLightColor { get; set; } // Couleur de la lumiere ambiante
        public Vector3D Light2Attenuation { get; set; } // Attenuation de la lumiere
        public Vector3D Light3Attenuation { get; set; } // Attenuation de la lumiere
        public bool RenderLight1 { get; set; } // Afficher la lumiere
        public bool RenderLight2 { get; set; } // Afficher la lumiere
        public bool RenderLight3 { get; set; } // Afficher la lumiere

        private bool renderDiffuseMap = true; // Afficher la texture diffuse

        public new event PropertyChangedEventHandler PropertyChanged;

        private double light1DirectionX;
        public double Light1DirectionX
        {
            get { return light1DirectionX; }
            set
            {
                if (light1DirectionX != value)
                {
                    light1DirectionX = value;
                    OnPropertyChanged("Light1DirectionX");
                    UpdateLight1Direction();
                }
            }
        }

        private double light1DirectionY;
        public double Light1DirectionY
        {
            get { return light1DirectionY; }
            set
            {
                if (light1DirectionY != value)
                {
                    light1DirectionY = value;
                    OnPropertyChanged("Light1DirectionX");
                    UpdateLight1Direction();
                }
            }
        }

        private double light1DirectionZ;
        public double Light1DirectionZ
        {
            get { return light1DirectionZ; }
            set
            {
                if (light1DirectionZ != value)
                {
                    light1DirectionZ = value;
                    OnPropertyChanged("Light1DirectionX");
                    UpdateLight1Direction();
                }
            }
        }

        private Vector3D light1Direction;
        public Vector3D Light1Direction
        {
            get { return light1Direction; }
            set
            {
                if (light1Direction != value)
                {
                    light1Direction = value;
                    OnPropertyChanged("Light1Direction");
                }
            }
        }

        private void UpdateLight1Direction()
        {
            Light1Direction = new Vector3D(Light1DirectionX, Light1DirectionY, Light1DirectionZ);
        }

        protected virtual new void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        // Afficher la texture diffuse
        public bool RenderDiffuseMap
        {
            set
            {
                if (SetValue(ref renderDiffuseMap, value))
                {
                    ModelMaterial.RenderDiffuseMap = FloorMaterial.RenderDiffuseMap = value;
                }
            }
            get { return renderDiffuseMap; }
        }

        private bool renderNormalMap = true; // Afficher la texture normale

        // Afficher la normale map de la texture
        public bool RenderNormalMap
        {
            set
            {
                if (SetValue(ref renderNormalMap, value))
                {
                    ModelMaterial.RenderNormalMap = FloorMaterial.RenderNormalMap = value;
                }
            }
            get { return renderNormalMap; }
        }

        // Liste des textures
        public string[] TextureFiles { get; } = new string[] { @"TextureCheckerboard2.jpg", @"TextureCheckerboard3.jpg", @"TextureNoise1.jpg", @"TextureNoise1_dot3.jpg", @"TextureCheckerboard2_dot3.jpg" };

        private string selectedDiffuseTexture = @"TextureCheckerboard2.jpg"; // Texture diffuse selectionnee

        // Texture diffuse selectionnee
        public string SelectedDiffuseTexture
        {
            set
            {
                if (SetValue(ref selectedDiffuseTexture, value))
                {
                    ModelMaterial.DiffuseMap = TextureModel.Create(new System.Uri(value, System.UriKind.RelativeOrAbsolute).ToString());
                    FloorMaterial.DiffuseMap = ModelMaterial.DiffuseMap;
                }
            }
            get
            {
                return selectedDiffuseTexture;
            }
        }

        private string selectedNormalTexture = @"TextureCheckerboard2_dot3.jpg"; // Texture normale selectionnee

        // Texture normale selectionnee
        public string SelectedNormalTexture
        {
            set
            {
                if (SetValue(ref selectedNormalTexture, value))
                {
                    ModelMaterial.NormalMap = TextureModel.Create(new System.Uri(value, System.UriKind.RelativeOrAbsolute).ToString());
                    FloorMaterial.NormalMap = ModelMaterial.NormalMap;
                }
            }
            get
            {
                return selectedNormalTexture;
            }
        }

        // Couleur du materiau
        public System.Windows.Media.Color FloorDiffuseColor
        {
            set
            {
                FloorMaterial.DiffuseColor = ModelMaterial.DiffuseColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.DiffuseColor.ToColor();
            }
        }

        // Couleur reflective du materiau
        public System.Windows.Media.Color FloorReflectiveColor
        {
            set
            {
                FloorMaterial.ReflectiveColor = ModelMaterial.ReflectiveColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.ReflectiveColor.ToColor();
            }
        }

        // Couleur speculaire du materiau
        public System.Windows.Media.Color FloorSpecularColor
        {
            set
            {
                FloorMaterial.SpecularColor = ModelMaterial.SpecularColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.SpecularColor.ToColor();
            }
        }

        // Couleur de la lumiere emissive
        public System.Windows.Media.Color FloorEmissiveColor
        {
            set
            {
                FloorMaterial.EmissiveColor = ModelMaterial.EmissiveColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.EmissiveColor.ToColor();
            }
        }

        // Creer une nouvelle instance de la camera
        public Camera Camera1 { get; } = new PerspectiveCamera { Position = new Point3D(0, 30, 0), LookDirection = new Vector3D(0, -1, 1), UpDirection = new Vector3D(0, 1, 0) };

        // Fonction principale de la scene
        public MainViewModel()
        {
            //RenderTechniquesManager = new DefaultRenderTechniquesManager();

            EffectsManager = new DefaultEffectsManager(); // Initialiser le gestionnaire d'effets


            // ----------------------------------------------
            // Titres dans la fenetre
            this.Title = "Demo Jerome";
            this.SubTitle = "WPF & SharpDX";


            // ----------------------------------------------
            // Setup de la camera
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 30, -30), LookDirection = new Vector3D(0, -1, 1), UpDirection = new Vector3D(0, 1, 0) };

            // ----------------------------------------------
            // Setup de la lumiere

            this.AmbientLightColor = Colors.DarkGray;

            this.RenderLight1 = true;
            this.RenderLight2 = false;
            this.RenderLight3 = false;

            this.Light1Color = Colors.White;
            this.Light2Color = Colors.Yellow;
            this.Light3Color = Colors.Red;
            this.Light2Attenuation = new Vector3D(0.001f, 0.001f, 0.001f);
            this.Light3Attenuation = new Vector3D(0.001f, 0.001f, 0.001f);

            this.Light1Direction = new Vector3D(-1, -1, 0);
            //this.Light1Transform = new TranslateTransform3D(Light1X, Light1Y, Light1Z);
            //this.Light1Transform = CreateAnimatedTransform1(Light1Direction, new Vector3D(0, -100, 0), 5);
            //this.Light2Transform = CreateAnimatedTransform1(new Vector3D(0, 0, 100), new Vector3D(0, -10, 0), 3);
            //this.Light3Transform = new TranslateTransform3D(0, 0, 0);
            //this.Light3Transform = CreateAnimatedTransform3(0, 100, 3);


            var transformGroup = new Media3D.Transform3DGroup();
            transformGroup.Children.Add(new Media3D.ScaleTransform3D(10, 10, 10));
            transformGroup.Children.Add(new Media3D.TranslateTransform3D(2, -4, 2));
            ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);


            // ----------------------------------------------
            // Modele 3D 

            //var sphere = new MeshBuilder();
            //sphere.AddSphere(new Vector3(0, 15, 0), 10);
            //Sphere = sphere.ToMeshGeometry3D();
            //this.LightModelMaterial = new PhongMaterial
            //{
            //    AmbientColor = Colors.Gray.ToColor4(),
            //    DiffuseColor = Colors.Gray.ToColor4(),
            //    EmissiveColor = Colors.Yellow.ToColor4(),
            //    SpecularColor = Colors.Black.ToColor4(),

            //};

            //var sphere2 = new MeshBuilder();
            //sphere2.AddSphere(new Vector3(0, 15, 0), 5);
            //Sphere2 = sphere2.ToMeshGeometry3D();
            //this.LightModelMaterial2 = new PhongMaterial
            //{
            //    AmbientColor = Colors.Gray.ToColor4(),
            //    DiffuseColor = Colors.Gray.ToColor4(),
            //    EmissiveColor = Colors.Red.ToColor4(),
            //    SpecularColor = Colors.Black.ToColor4(),

            //};

            //var sphere2 = new MeshBuilder();
            //sphere2.AddSphere(new Vector3(0, 30, 0), 10);
            //Sphere2 = sphere2.ToMeshGeometry3D();

            //// Creer un materiau reflechissant
            //ReflectMaterial = PhongMaterials.Chrome;
            //ReflectMaterial.ReflectiveColor = global::SharpDX.Color.White;
            //ReflectMaterial.RenderEnvironmentMap = true;

            // ----------------------------------------------
            // scene model3d

            var b1 = new MeshBuilder(true, true, true);
            b1.AddBox(new Vector3(0.0f, 22.5f, 0.0f), 50, 50, 50, BoxFaces.All);
            //b1.AddSphere(new Vector3(-75f, 10f, -50f), 15, 24, 24);
            //b1.AddSphere(new Vector3(-60f, 25f, -40f), 5, 24, 24);
            //b1.AddTetrahedron(new Vector3(110f, 0f, 0f), new Vector3(5, 0, 0), new Vector3(0, 5, 0), 5);
            //b1.AddPipe(new Vector3(0, -5, 0), new Vector3(0, 100, 0), 25, 30, 36);
            this.Model = b1.ToMeshGeometry3D();
            this.ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.ModelMaterial = new PhongMaterial();
            this.ModelMaterial = new PhongMaterial
            {
                AmbientColor = Colors.DarkGray.ToColor4(),
                DiffuseColor = Colors.DarkGray.ToColor4(),
                SpecularColor = Colors.White.ToColor4(),
                ReflectiveColor = Colors.White.ToColor4(),
                SpecularShininess = 10f, // Plus faible = brillance plus forte - Plus fort = brillance plus faible
                DiffuseMap = TextureModel.Create(new System.Uri(SelectedDiffuseTexture, System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = ModelMaterial.NormalMap,
                RenderShadowMap = true
            };
            ModelMaterial.DiffuseMap = ModelMaterial.DiffuseMap;

            //this.ModelMaterial = PhongMaterials.White;
            //this.ModelMaterial.NormalMap = TextureModel.Create(new System.Uri(SelectedNormalTexture, System.UriKind.RelativeOrAbsolute).ToString());


            // ----------------------------------------------
            // Modele 3D du sol

            var b2 = new MeshBuilder(true, true, true);
            b2.AddBox(new Vector3(0.0f, -7.5f, 0.0f), 500, 10, 500, BoxFaces.All);
            this.Floor = b2.ToMeshGeometry3D();
            this.FloorTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.FloorMaterial = new PhongMaterial
            {
                AmbientColor = Colors.DarkGray.ToColor4(),
                DiffuseColor = Colors.DarkGray.ToColor4(),
                SpecularColor = Colors.White.ToColor4(),
                ReflectiveColor = Colors.Red.ToColor4(),
                SpecularShininess = 100f, // Plus faible = brillance plus forte - Plus fort = brillance plus faible
                DiffuseMap = TextureModel.Create(new System.Uri(SelectedDiffuseTexture, System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = ModelMaterial.NormalMap,
                RenderShadowMap = true
            };
            ModelMaterial.DiffuseMap = FloorMaterial.DiffuseMap;


            //InitialObjectTransforms();
        }

        //private void InitialObjectTransforms()
        //{
        //    var b = new MeshBuilder(true);
        //    b.AddTorus(1, 0.5);
        //    b.AddTetrahedron(new Vector3(), new Vector3(1, 0, 0), new Vector3(0, 1, 0), 1.1);
        //    var random = new Random();
        //    Object1Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-5, 5)),
        //        new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-5, 5)), random.NextDouble(2, 10));
        //}


        // ----------------------------------------------
        // Animation de rotation autour d'un axe

        private Media3D.Transform3D CreateAnimatedTransform1(Vector3D translate, Vector3D axis, double speed)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 180),
                Duration = TimeSpan.FromSeconds(speed),
                IsCumulative = true,
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);

            return lightTrafo;
        }

        // ----------------------------------------------
        // Animation 

        private Media3D.Transform3D CreateAnimatedTransform2(Vector3D translate, Vector3D axis, double speed)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 180),
                From = new Media3D.AxisAngleRotation3D(axis, 135),
                To = new Media3D.AxisAngleRotation3D(axis, 225),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed),
                IsCumulative = true,
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);
            return lightTrafo;
        }


        // ----------------------------------------------
        // Animation de translation
        private Media3D.Transform3D CreateAnimatedTransform3(double start, double finish, double speed)
        {
            var lightTrafo = new Media3D.Transform3DGroup();

            var translateTransform = new Media3D.TranslateTransform3D();
            lightTrafo.Children.Add(translateTransform);

            var translateAnimation = new DoubleAnimation
            {
                From = start,
                To = finish, // Change this value to control the height of the up and down movement
                Duration = TimeSpan.FromSeconds(speed),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            translateTransform.BeginAnimation(Media3D.TranslateTransform3D.OffsetYProperty, translateAnimation);

            return lightTrafo;
        }


    }
}
