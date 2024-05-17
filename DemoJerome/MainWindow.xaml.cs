using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoJerome
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            Closed += (s, e) => {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();
                }
            };
        }
        //private void Window_KeyDown(object sender, KeyEventArgs e)
        //{
        //    var moveVector = new Vector3D();
        //    var rotationQuaternion = new Quaternion();

        //    var camera = view1.Camera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
        //    if (camera == null) return;

        //    switch (e.Key)
        //    {
        //        case Key.W:
        //            moveVector += camera.LookDirection; // Avancer
        //            break;
        //        case Key.S:
        //            moveVector -= camera.LookDirection; // Reculer
        //            break;
        //        case Key.Q:
        //            var rightDirection = Vector3D.CrossProduct(camera.LookDirection, camera.UpDirection);
        //            rotationQuaternion *= new Quaternion(rightDirection, -1); // Tourner à gauche
        //            break;
        //        case Key.D:
        //            rightDirection = Vector3D.CrossProduct(camera.LookDirection, camera.UpDirection);
        //            rotationQuaternion *= new Quaternion(rightDirection, 1); // Tourner à droite
        //            break;
        //    }

        //    // Appliquer la rotation à la caméra
        //    var axisAngleRotation3D = new AxisAngleRotation3D(rotationQuaternion.Axis, rotationQuaternion.Angle);
        //    var rotateTransform3D = new RotateTransform3D(axisAngleRotation3D);
        //    camera.LookDirection = rotateTransform3D.Transform(camera.LookDirection);

        //    // Appliquer le mouvement à la caméra
        //    camera.Position += moveVector;
        //}



    }
}
