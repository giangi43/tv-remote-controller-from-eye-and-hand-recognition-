using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.Structure;
using Emgu.CV;
using HandGestureRecognition.SkinDetector;
using Emgu.CV.CvEnum;

namespace HandGestureRecognition
{
    public partial class Form1 : Form
    {
        MCvAvgComp[] iieyes1;
        MCvAvgComp[] iifaces;
        Point[] momo = new Point[4];
        Point[] momo1 = new Point[4];
        bool maronne;
        int time = System.DateTime.Now.Second;
        int scount = System.DateTime.Now.Second;
        //int comando = 0, comandohot = 0, vcomandohot=0;
        //int vcomando = 0, pvcomando = 0,balletto=0;
        //int tcomando = 0,nonna=0;

        int mela = 0;
        Image<Gray, byte> iigrayframe;///♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ 
        Rectangle iiprevious;//♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪
        IColorSkinDetector skinDetector;
        public HaarCascade iihaarface;
        public HaarCascade iihaareye;
        bool ii = false;
        Image<Gray, byte> iigrayaux1;
       
        int iicoso = 0;
        int tormbolla = 0;

        Image<Bgr, Byte> currentFrame;
        Image<Bgr, Byte> currentFrameCopy;
                
        Capture grabber;
        AdaptiveSkinDetector detector;
        
        int frameWidth;
        int frameHeight;
        
        Hsv hsv_min;
        Hsv hsv_max;
        Ycc YCrCb_min;
        Ycc YCrCb_max;
        
        Seq<Point> hull;
        Seq<Point> filteredHull;
        Seq<MCvConvexityDefect> defects;
        MCvConvexityDefect[] defectArray;
        
        MCvBox2D box;
        
        MemStorage storage = new MemStorage();

       

        
        public int s = 0;

        

        public Form1()
        {
            InitializeComponent();
            //if (comboBox1.SelectedText == "")
            //{
            //    serialPort1.PortName = "COM13";
            //}
            //else
            //{
            //    serialPort1.PortName = comboBox1.SelectedText;
            //}
            //serialPort1.BaudRate = 9600;
            //serialPort1.Open();

            //grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_BRIGHTNESS, 1.8);

            if (grabber == null)
            {
                comboBox2.Items.Insert(0, 0);
                comboBox2.Items.Insert(1, 1);

                
                try
                {

                    //grabber = new Emgu.CV.Capture("rtsp://192.168.100.94:8554/");
               
                   
                    grabber = new Emgu.CV.Capture(0); // inizializza la variabile di tipo capture e sceglie la camera da cui prendere l'immagine 
                    
                 }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }

            grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 1080);
            grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1920);
            //grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS, 10);
            grabber.FlipHorizontal = true;
           
            this.Width = (grabber.Width + grabber.Width)/2+20;
            this.Height = (grabber.Height)/2+40;
            
            frameWidth = grabber.Width;
            frameHeight = grabber.Height;
            Point p = new Point(0, 0);


            //MCvAvgComp[] iifaces;
           
            detector = new AdaptiveSkinDetector(1, AdaptiveSkinDetector.MorphingMethod.NONE);
            hsv_min = new Hsv(0, 45, 0); 
            hsv_max = new Hsv(2,0,255);
            YCrCb_min = new Ycc(0, 131, 80); //new Ycc(0, 131, 80);
            YCrCb_max = new Ycc(255, 210, 135); //new Ycc(255, 185, 135);
            box = new MCvBox2D();
           
            iihaarface = new HaarCascade("haarcascade_frontalface_alt_tree.xml");
            iihaareye = new HaarCascade("haarcascade_eye.xml");


            Application.Idle += new EventHandler(FrameGrabber);    
                    
        }

        private void ereseFace(MCvAvgComp[] iifaces) // cancella i volti per il metodo skin.detector()
        {
            if (iifaces.Length != 0)
            {
                foreach (var face in iifaces)
                {
                    iiprevious = face.rect;
                    iiProcessImage(face.rect, currentFrame);



                    momo[0].X = face.rect.Left;
                    momo[0].Y = face.rect.Top;

                    momo[1].X = face.rect.Left;
                    momo[1].Y = face.rect.Bottom;

                    momo[2].X = face.rect.Right;
                    momo[2].Y = face.rect.Bottom;

                    momo[3].X = face.rect.Right;
                    momo[3].Y = face.rect.Top;

                    currentFrameCopy.FillConvexPoly(momo, new Bgr(Color.Black));
                }
            }
            else
            {
                iiProcessImage(iiprevious, currentFrame);
            }
            #region erese if
            //if (iifaces.Length != 0)
            //{

            //    iiprevious = iifaces[0].rect;
            //    iiProcessImage(iifaces[0].rect, currentFrame);



            //    momo[0].X=iifaces[0].rect.Left;
            //    momo[0].Y=iifaces[0].rect.Top;

            //    momo[1].X=iifaces[0].rect.Left;
            //    momo[1].Y=iifaces[0].rect.Bottom;

            //    momo[2].X=iifaces[0].rect.Right;
            //    momo[2].Y=iifaces[0].rect.Bottom;

            //    momo[3].X=iifaces[0].rect.Right;
            //    momo[3].Y=iifaces[0].rect.Top;

            //    if (iifaces.Length > 1)
            //    {
            //        iiprevious = iifaces[1].rect;
            //        iiProcessImage(iifaces[1].rect, currentFrame);

            //        momo1[0].X = iifaces[0].rect.Left;
            //        momo1[0].Y = iifaces[0].rect.Top;

            //        momo1[1].X = iifaces[0].rect.Left;
            //        momo1[1].Y = iifaces[0].rect.Bottom;

            //        momo1[2].X = iifaces[0].rect.Right;
            //        momo1[2].Y = iifaces[0].rect.Bottom;

            //        momo1[3].X = iifaces[0].rect.Right;
            //        momo1[3].Y = iifaces[0].rect.Top;
            //    }
            //    else 
            //    {
            //        momo1[0].X = 0;
            //        momo1[0].Y = 0;

            //        momo1[1].X = 0;
            //        momo1[1].Y = 0;

            //        momo1[2].X = 0;
            //        momo1[2].Y = 0;

            //        momo1[3].X = 0;
            //        momo1[3].Y = 0;  
            //    }



            //}
            //else
            //{
            //    momo[0].X = 0;
            //    momo[0].Y = 0;

            //    momo[1].X = 0;
            //    momo[1].Y = 0;

            //    momo[2].X = 0;
            //    momo[2].Y = 0;

            //    momo[3].X = 0;
            //    momo[3].Y = 0;

            //    momo1 [ 0].X = 0;
            //    momo1[ 0].Y = 0;

            //    momo1[ 1].X = 0;
            //    momo1[ 1].Y = 0;

            //    momo1[ 2].X = 0;
            //    momo1[ 2].Y = 0;

            //    momo1[ 3].X = 0;
            //    momo1[ 3].Y = 0;

            //    iiProcessImage(iiprevious, currentFrame);


            //}

            //currentFrameAno.FillConvexPoly(momo, new Bgr(Color.Black));
            //currentFrameAno.FillConvexPoly(momo1, new Bgr(Color.Black));
            #endregion
        }

        private void fps() // mostra i fps
        {
            if (time == System.DateTime.Now.Second)
                mela++;
            else
            {                
                label1.Text = Convert.ToString(mela) + " FPS";
                mela = 0;
                time = System.DateTime.Now.Second;
            }
        }

        protected void FrameGrabber(object sender, EventArgs e)
        {
            fps();
                   
            try 
            {
                //System.Threading.Thread.Sleep(1);

                currentFrame = grabber.QueryFrame(); // nuova cattura da fotocamera   
                
            }
            catch (AccessViolationException excpt)
            {
                MessageBox.Show(excpt.Message);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

        

        

            if (currentFrame != null)
            {

               

               currentFrameCopy = currentFrame.Clone();

               try
               {
                   iigrayframe = currentFrame.Convert<Gray, byte>(); //♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫
               }
               catch (NotSupportedException ex)
               {
                   MessageBox.Show(ex.Message);
                   Application.Exit();
               }
               catch (Exception ex)
               {
                   MessageBox.Show(ex.Message);
                   Application.Exit();
               }

               try
               {

                  iifaces = iigrayframe.DetectHaarCascade(
                                           iihaarface, 1.1, 3,//1.4, 4,
                                           HAAR_DETECTION_TYPE.DEFAULT,
                                           new Size(currentFrame.Width / 5, currentFrame.Height / 4))[0];//♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ ♪ ♫ 
               }
               catch (Exception ex)
               {
                   //Application.Exit();
                   MessageBox.Show(ex.Message);
                   Application.Restart();
               }
                
                label4.Text = iifaces.Length.ToString();

                ereseFace(iifaces);
               
                skinDetector = new YCrCbSkinDetector();

                Image<Gray, Byte> skin = skinDetector.DetectSkin(currentFrameCopy, YCrCb_min, YCrCb_max);

                ExtractContourAndHull(skin);
                                
                DrawAndComputeFingersNum();

                Point p = new Point(0, 0);

                imageBoxFrameGrabber.SetZoomScale(0.5, p);
                imageBoxSkin.SetZoomScale(0.5, p);
                
                imageBoxSkin.Image = skin;
                imageBoxFrameGrabber.Image = currentFrame;
                
            }
        }

        private void timEye() // scrive da quanto non vengono visti gli occhi e sincronizza la funzione telecomando
        {
            //if (time == System.DateTime.Now.Second)
                //    mela++;
                //else
                //{
                //    //grabber = new Emgu.CV.Capture("rtsp://192.168.100.74:8554/");
                //    label1.Text = Convert.ToString(mela) + " FPS";
                //    mela = 0;
                //    time = System.DateTime.Now.Second;
                //}


                if (iieyes1.Length == 0 )
                {
                    maronne=true;
                    //iicoso++;
                    

                }
                else
                { 
                    maronne=false;
                    //iicoso = 0;
                    label2.Text = iicoso.ToString();
                    
                }
                if (scount != System.DateTime.Now.Second)
                {
                    //tcomando++;
                    if (maronne)
                    {
                        //scount = System.DateTime.Now.Second;
                        iicoso++;
                        label2.Text = iicoso.ToString() + " s senza vedere occhi";
                        //    if (balletto == 4)
                        //    {
                        //        balletto = 0;
                        //    }
                        //}
                        //else if (balletto == 3 && !maronne)
                        //{
                        //    if (iicoso > 1)
                        //    { iicoso = 0; }
                        //    iicoso = iicoso - 1;
                    }
                    else
                    { iicoso = 0; }

                    scount = System.DateTime.Now.Second;
                }
                label3.Text = tormbolla.ToString() + " beep";
        }

         #region iiProcessImage;

        private void iiProcessImage(Rectangle iiface, Image<Bgr, byte> currentFrame)
        {
            if ((ii == false) && (iiface.Width != 0))
                ii = true;

            if (ii)
            {


                if (iifaces.Length > 0)
                {
                    //Rectangle roi = new Rectangle();+ iiface.Height/4 
                    Rectangle iiroi = new Rectangle(iiface.Left, iiface.Top, iiface.Width, iiface.Height / 5);
                    iigrayaux1 = currentFrame.Copy(new Rectangle(iiroi.Left, iiroi.Top, iiroi.Width, iiroi.Height)).Convert<Gray, byte>();
                    iieyes1 = iigrayaux1.DetectHaarCascade(
                                 iihaareye, 1.4, 5,
                                 HAAR_DETECTION_TYPE.DEFAULT,
                                 new Size(iiroi.Width / 10, iiroi.Height / 5))[0];
                    //new Size((grabber.Width / 4) / 7, (grabber.Height / 4) / 2))[0];
                }
                else
                {
                    iigrayaux1 = currentFrame.Convert<Gray, byte>();
                    iieyes1 = iigrayaux1.DetectHaarCascade(
                                iihaareye, 1.4, 5,
                                HAAR_DETECTION_TYPE.DEFAULT,
                                new Size((grabber.Width / 5) / 7, (grabber.Height / 5) / 2))[0];
                }




                timEye();
                if (checkBox1.Checked == true)
                {
                    foreach (var eye in iieyes1)
                    {
                        PointF eyep = new PointF(eye.rect.Left + (eye.rect.Width / 2), eye.rect.Top + (eye.rect.Height / 2));
                        CircleF eyelimit = new CircleF(eyep, eye.rect.Width / 5);
                        //currentFrame.Draw(eye.rect, new Bgr(Color.GreenYellow), 3);
                        currentFrame.Draw(eyelimit, new Bgr(Color.Red), 2);
                        //Emgu.CV.Structure.
                    }
                }
            }
                
            
        }

         #endregion


        #region ExtractContourAndHull
        private void ExtractContourAndHull(Image<Gray, byte> skin)
        {
           
            

                Contour<Point> contours = skin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage);
                Contour<Point> biggestContour = null;

                Double Result1 = 0;
                Double Result2 = 0;
                while (contours != null)
                {
                    Result1 = contours.Area;
                    if (Result1 > Result2)
                    {
                        Result2 = Result1;
                        biggestContour = contours;
                    }
                    contours = contours.HNext;
                }

                if (biggestContour != null)
                {
                    
                    Contour<Point> currentContour = biggestContour.ApproxPoly(biggestContour.Perimeter * 0.0025, storage);
                    currentFrame.Draw(currentContour, new Bgr(Color.LimeGreen), 4);
                    biggestContour = currentContour;
                

                    hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    box = biggestContour.GetMinAreaRect();
                    PointF[] points = box.GetVertices();
                   
                    Point[] ps = new Point[points.Length];
                    for (int i = 0; i < points.Length; i++)
                        ps[i] = new Point((int)points[i].X, (int)points[i].Y);

                    currentFrame.DrawPolyline(hull.ToArray(), true, new Bgr(Color.White), 4);
                    currentFrame.Draw(new CircleF(new PointF(box.center.X, box.center.Y), 6), new Bgr(200, 125, 75), 2);

                  
                   

                    filteredHull = new Seq<Point>(storage);
                    for (int i = 0; i < hull.Total; i++)
                    {
                        if (Math.Sqrt(Math.Pow(hull[i].X - hull[i + 1].X, 2) + Math.Pow(hull[i].Y - hull[i + 1].Y, 2)) > box.size.Width / 10)
                        {
                            filteredHull.Push(hull[i]);
                        }
                    }

                    defects = biggestContour.GetConvexityDefacts(storage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                   
                    defectArray = defects.ToArray();
                }
            
        }
        #endregion

        private void DrawAndComputeFingersNum()
        {
            int fingerNum = 0;

           #region defects drawing
            if (defects != null)
            {
                for (int i = 0; i < defects.Total; i++)
                {
                    PointF startPoint = new PointF((float)defectArray[i].StartPoint.X,
                                                    (float)defectArray[i].StartPoint.Y);

                    PointF depthPoint = new PointF((float)defectArray[i].DepthPoint.X,
                                                    (float)defectArray[i].DepthPoint.Y);

                    PointF endPoint = new PointF((float)defectArray[i].EndPoint.X,
                                                    (float)defectArray[i].EndPoint.Y);

                    LineSegment2D startDepthLine = new LineSegment2D(defectArray[i].StartPoint, defectArray[i].DepthPoint);

                    LineSegment2D depthEndLine = new LineSegment2D(defectArray[i].DepthPoint, defectArray[i].EndPoint);

                    CircleF startCircle = new CircleF(startPoint, 5f);

                    CircleF depthCircle = new CircleF(depthPoint, 5f);

                    CircleF endCircle = new CircleF(endPoint, 5f);

                    
                    if (((startDepthLine.Length > 50) && (depthEndLine.Length > 50)) || ((startDepthLine.Length > 20) && (depthEndLine.Length > 70)) || ((startDepthLine.Length > 70) && (depthEndLine.Length > 20)))
                    {

                        if ((startCircle.Center.Y < box.center.Y || depthCircle.Center.Y < box.center.Y) && (startCircle.Center.Y < depthCircle.Center.Y) && (Math.Sqrt(Math.Pow(startCircle.Center.X - depthCircle.Center.X, 2) + Math.Pow(startCircle.Center.Y - depthCircle.Center.Y, 2)) > box.size.Height / 6.5))
                        {
                            fingerNum++;
                            currentFrame.Draw(startDepthLine, new Bgr(Color.Green), 2);
                            currentFrame.Draw(depthEndLine, new Bgr(Color.Magenta), 2);
                        }
                    }

                    currentFrame.Draw(startCircle, new Bgr(Color.Red), 2);
                    currentFrame.Draw(depthCircle, new Bgr(Color.Yellow), 5);
                    currentFrame.Draw(endCircle, new Bgr(Color.DarkBlue), 4);
                }

            #endregion

                MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_DUPLEX, 7d, 10d);
                currentFrame.Draw(fingerNum.ToString(), ref font, new Point(80, 330), new Bgr(Color.GreenYellow));
                //telecomado(fingerNum);
                defects.Clear();
                storage.Clear();
            }
        }

        //string blocco="";
        #region telecomando
        //private void telecomado(int dita)
        //{
         
        //    comando = dita;


        //    if (iicoso > 3 && balletto == 0)
        //    { balletto = 1; }           
        //    else if (iicoso > 100000 && balletto == 3)
        //    { balletto = 4; }

        //    if (balletto == 1)
        //    { 
        //        serialPort1.Write("O");
        //        tcomando = 0;
        //        balletto = 3;
            
        //    }
        //    else if (balletto == 3 && iicoso < -5)
        //    {
        //        serialPort1.Write("O");
        //        tcomando = 0;
        //        balletto = 0;
                
            
        //    }



        //    if (comando == vcomando || pvcomando==comando || vcomando==pvcomando )
        //    {
        //        pvcomando = vcomando;
        //        vcomando = comando;

                

        //        if (comando == vcomando)
        //            comandohot = comando;
        //        else if (pvcomando == comando)
        //            comandohot = comando;
        //        else if (vcomando == pvcomando)
        //            comandohot = vcomando;

        //        if (comandohot == vcomandohot)
        //        {

        //            label5.Text = "comando " + comandohot + " " + tcomando;



        //            if (comandohot == 1)
        //            {

        //                if (comandohot == 1 && tcomando > 4 && blocco != "1")
        //                {
        //                    serialPort1.Write("p"); //play&pausa
        //                    blocco = "1";
        //                    tcomando = 0;

        //                }



        //            }
        //            else if (comandohot == 2)
        //            {
        //                if (comandohot != 2 && tcomando > 4 && blocco != "2")
        //                {
        //                    serialPort1.Write("v"); //voume meno
        //                    blocco = "2";
        //                    tcomando = 0;

        //                }



        //            }
        //            else if (comandohot == 3)
        //            {
        //                if (comandohot == 3 && tcomando > 4 && blocco != "3")
        //                {
        //                    serialPort1.Write("V"); //volume piu
        //                    blocco = "3";
        //                    tcomando = 0;

        //                }

        //            }
        //            else if (comandohot == 4 )
        //            {
        //                if ((comandohot == 4 && tcomando > 4)  && blocco != "4")
        //                {
        //                    serialPort1.Write("O"); //on&offffff
        //                    blocco = "4";
        //                    tcomando = 0;

        //                    balletto = 2;
        //                }



        //            }
        //            else if (comandohot == 5)
        //            {
        //                if (comandohot == 5 && tcomando > 4 && blocco != "5")
        //                {
        //                    serialPort1.Write("P"); //play .. di nuovo...
        //                    blocco = "5";
        //                    tcomando = 0;

        //                }


        //            }
        //        }

        //        else
        //        {
        //            vcomandohot = comandohot;
        //            tcomando = 0;
        //        }

        //        //tcomando = System.DateTime.Now.Second;
               
                
        //    }

        //    else
        //        tcomando = 4;
        //        pvcomando = vcomando;
        //        vcomando = comando;                
            
        //}

        #endregion

        private void imageBoxFrameGrabber_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            s++;
            currentFrame.Save(@"tunz" + s.ToString()+".jpg");
            button1.Text = "salva " + s.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
             
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            grabber = new Capture(comboBox2.SelectedIndex);
            grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 1080);
            grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1920);
            //grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS, 10);
            grabber.FlipHorizontal = true;
            
            //System.Threading.Thread.Sleep(10);
        }
                                      
    }
}