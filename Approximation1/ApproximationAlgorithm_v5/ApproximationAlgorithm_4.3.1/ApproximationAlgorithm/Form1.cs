using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;



namespace ApproximationAlgorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           /* textBox2.Text = " ";
            textBox1.Text = " ";*/
        }

        List<Node> boxnode = new List<Node>();//для записи  считанных из файла исходных координат
        List<Node> OutputNode = new List<Node>(); // для записи выходной последовательности узлов
        List<double> boxcurv = new List<double>(); // список кривизн 1...(n-1)
        double ep = 0;

        string path = ""; // для записи пути
        string filename = ""; // для записи имени файла

        // считывание данных об узлах из файла
        private void ReadNode()
        {
            double X;
            double Y;
            double value = 0;
            if(openFileDialog1.ShowDialog()== DialogResult.OK)
            {
                path = Path.GetDirectoryName(openFileDialog1.FileName);
                filename = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
               
                //filename = Path.GetFileName(openFileDialog1.FileName);
                
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    string line = "";
                    string line1 = "";
                    bool result = false;
                    while(sr.EndOfStream == false)
                    {
                        line = sr.ReadLine();
                        if(string.IsNullOrEmpty(line))
                        {
                            MessageBox.Show("Ошибка чтения данных из файла 1");
                            sr.Close();
                            return;
                        }
                        int index1 = line.IndexOf(" ");
                        if(index1==-1|| index1+1>=line.Length)
                        {
                            MessageBox.Show("Ошибка чтения данных из файла 2");
                            sr.Close();
                            return;
                        }

                        line1 = line.Substring(0, index1);
                        line1.Trim();
                        result = Double.TryParse(line1,out value);
                        if(result == true)
                        {
                            X = value;
                           
                        }
                        else
                        {
                            MessageBox.Show("Ошибка чтения данных из файла 3");
                            sr.Close();
                            return;
                                
                        }

                        // Запись координаты Y
                        line1 = line.Substring(index1+1,line.Length-(index1+1));
                        line1.Trim();
                        result = Double.TryParse(line1,out value);
                        if(result == true)
                        {
                            Y = value;
                        }
                         else
                        {
                            MessageBox.Show("Ошибка чтения данных из файла 3");
                            sr.Close();
                            return;

                        }
                        
                        Node p = new Node(X, Y);
                     
                        boxnode.Add(p);
                        p.Index = boxnode.Count - 1;

                    }

                     // В постановке задачи 1-ый и последний узлы ломаной не совпадают (A1,A2,...,An, n>1),у меня в некоторых
                     //местах это не так. Надо бы исправить или не надо 
                    if(boxnode.Count == 1)
                    {
                        MessageBox.Show("Количество узлов исходной линии равно 1!"+ "\n"+
                            "Для работы алгоритма число узлов должно быть>1! Алгоритм не быдет выполнен!");
                        return;
                        
                    }

                  /*  if(boxnode[0].X == boxnode[boxnode.Count-1].X && boxnode[0].Y == boxnode[boxnode.Count - 1].Y)
                    {
                        // для отладки - как сравниваются числа с плавающей запятой?
                        MessageBox.Show("Первый и последний узлы сопадают!");
                        boxnode.RemoveAt(boxnode.Count - 1);

                    }*/

                   // MessageBox.Show(boxnode.Count.ToString());

                }

            }

            else
            {
                return;
            }
            
      }


        //Удовлетворяет ли узел условию (*)  (Определение 1)
        private bool Uslovie(int k, List<Node> somenode)
        {
            bool b = false;
            for (int i = 1; i < k; i++)
            {
                b = this.Perpendicular(somenode[0],somenode[k], somenode[i]);
                if(b == false)
                {
                    return false;
                }

            }

            return true;

        }

        //Проверка попадания основания 
        //перпендикуляра на отрезок, а не на его продолжение(Предложение 1)
        private bool Perpendicular(Node A, Node B, Node U)
        {
            //bool result = false;// для записи результата (true - лежит на основании, false - не лежит)
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double x = U.X;
            double y = U.Y;

            /* double z0 = (U.X - A.X) * (B.X - A.X) + (U.Y - A.Y) * (B.Y - A.Y);
             double z1 = (U.X - B.X) * (B.X - A.X) + (U.Y - B.Y) * (B.Y - A.Y);*/
            double z0 = (x - s0) * (s1 - s0) + (y - t0) * (t1-t0);
            double z1 = (x - s1) * (s1 - s0) + (y - t1) * (t1-t0);
            if (z0 == 0 || z1 == 0 || z0 * z1 < 0)
            {
                return true;//лежит на орезке, а не на его продолжении
            }
            else
                return false;// не лежит на отрезке
            
        }

        // Вычисление координат основания перпендикуляра (Предложение 1)
        private Node BasePerpendicular(Node A, Node B, Node U)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double x = U.X;
            double y = U.Y;

            double zn = Math.Pow(t1 - t0, 2) + Math.Pow(s1 - s0, 2);
            double u = (s0 * Math.Pow(t1 - t0, 2)) / zn +
                ((y - t0) * (s1 - s0) * (t1 - t0)) / zn +
                (x * Math.Pow(s1 - s0, 2)) / zn;
            double v = (y * Math.Pow(t1 - t0, 2)) / zn +
                ((x - s0) * (s1 - s0) * (t1 - t0)) / zn +
                (t0 * Math.Pow(s1 - s0, 2)) / zn;
            Node n = new Node(u, v);
            return n;

            /*double u = (A.X * Math.Pow((B.Y - A.Y), 2)) / (Math.Pow((B.Y - A.Y), 2) + Math.Pow((B.X - A.X), 2)) +
                ((U.Y - A.Y) * (B.X - A.X) * (B.Y - A.Y)) / (Math.Pow((B.Y - A.Y), 2) + Math.Pow((B.X - A.X), 2)) +
                (U.X * Math.Pow((B.X - A.X), 2)) / (Math.Pow((B.Y - A.Y), 2) + Math.Pow((B.X - A.X), 2));

            double v= (U.Y* Math.Pow((B.Y - A.Y), 2))/ (Math.Pow((B.Y - A.Y), 2) + Math.Pow((B.X - A.X), 2))+
                ((U.X-A.X)*(B.X-A.X)*(B.Y-A.Y))/ (Math.Pow((B.Y - A.Y), 2) + Math.Pow((B.X - A.X), 2))+
                A.Y*Math.Pow((B.X-),2)*/

        }

        // Расстояние от точки до отрезка (Формула 1)
        private double Dist(Node A, Node B, Node U)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double x = U.X;
            double y = U.Y;

            double d = Math.Abs(((t1 - t0) * (x - s0) + (s1 - s0) * (t0 - y)) / Math.Sqrt(Math.Pow(t1 - t0, 2) + Math.Pow(s1 - s0, 2)));

            return d;
        }

        // НЕ Лежат ли узла на одной прямой (Предложение 4)
        private bool NodeOnLine(Node A, Node B, Node C)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double s2 = C.X;
            double t2 = C.Y;
            if ((s1 - s0) * (t2 - t0) != (t1 - t0) * (s2 - s0))
            {
                return true;// не лежат на одной прямой
            }

            else
                return false; // лежат на одной прямой
            
        }

        // НЕ Острый ли угол
        private bool Angle(Node A, Node B, Node U)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double x = U.X;
            double y = U.Y;

            if ((s0 - x) * (s1 - x) + (t0 - y) * (t1 - y) <= 0)
            {
                return true; // т.е. угол не острый
            }

            else return false; // угол острый
        
        }

        // Вычисление координат центра окружности (Формула 2)
         private Node Circle(Node A, Node B, Node C)
        {
            /*double s0 = Math.Round(A.X,2);
            double t0 = Math.Round(A.Y,2);
            double s1 = Math.Round(B.X,2);
            double t1 = Math.Round(B.Y,2);
            double s2 = Math.Round(C.X,2);
            double t2 = Math.Round(C.Y,2);*/
           double s0 = A.X;
           double t0 = A.Y;
           double s1 = B.X;
           double t1 = B.Y;
           double s2 = C.X;
           double t2 = C.Y;
           
            double Z_s = (t0 - t1) * (s2 * s2 + t2 * t2) +
                (t1 - t2) * (s0 * s0 + t0 * t0) + (t2 - t0) * (s1*s1+t1*t1);
            double Z_t = (s0 - s1) * (s2 * s2 + t2 * t2) +
                (s1 - s2) * (s0 * s0 + t0 * t0) + (s2 - s0) * (s1*s1+t1*t1);
            double Z = (s0 - s1) * (t2 - t0) - (t0 - t1) * (s2 - s0);
           double c0 = -(Z_s / (2 * Z));
            //double d0 = -(Z_t / (2 * Z));
            double d0 = (Z_t / (2 * Z));
            Node  O = new Node(c0, d0);
            return O;

        }

        // Вычисление радиуса окружности (Формула 2.1)
        private double Circle_R(Node A, Node B, Node C)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double s2 = C.X;
            double t2 = C.Y;

           /* double s0 = Math.Round(A.X, 2);
            double t0 = Math.Round(A.Y, 2);
            double s1 = Math.Round(B.X, 2);
            double t1 = Math.Round(B.Y, 2);
            double s2 = Math.Round(C.X, 2);
            double t2 = Math.Round(C.Y, 2);*/

            double a = Math.Pow(s1 - s0, 2) + Math.Pow(t1 - t0, 2);
            double b = Math.Pow(s2 - s1, 2) + Math.Pow(t2 - t1, 2);
            double c = Math.Pow(s2 - s0, 2) + Math.Pow(t2 - t0, 2);

           double R = Math.Sqrt((a * b * c) / (2 * (a * b + a * c + b * c) - (a * a + b * b + c * c)));

            return R;


        }


        // Лежит ли узел в том же угле, что и дуга (Предложение 3)
        private bool NodeInAngle(Node O, Node A, Node B, Node C, Node M)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double s2 = C.X;
            double t2 = C.Y;
            double c0 = O.X;
            double d0 = O.Y;
            double x = M.X;
            double y = M.Y;

            double Z_AB = (d0 - t2) * (s1 - s2) - (c0 - s0) * (t1 - t0);
            double Z_AM = (d0 - t0) * (x - s0) - (c0 - s0) * (y-t0);
            double Z_CB = (d0 - t2) * (s1 - s2) - (c0 - s2) * (t1 - t2);
            double Z_CM = (d0 - t2) * (x - s2) - (c0 - s2) * (y-t2);

            if (Z_AB * Z_AM >= 0 & Z_CB * Z_CM >= 0)
            {
                return true; // точка лежит в том же угле
            }
            else return false; // не лежит в том же угле
                
        }


        // Определение 2
        private bool TrineNodex(int index1, int index_r,int index_p, List<Node> somebox)
        {
            bool a1 = this.NodeOnLine(somebox[index1], somebox[index_r], somebox[index_p]);
            if(a1 == false)
            {
                return false;
            }
            bool a2 = this.Angle(somebox[index1], somebox[index_p], somebox[index_r]);
            if (a2 == false)
            {
                return false;
            }
             
         
           Node O =  this.Circle(somebox[index1], somebox[index_r], somebox[index_p]);
            //Node O = this.Center(somebox[index1], somebox[index_r], somebox[index_p]);

            for (int i = index1+1; i < index_p; i++)
            {
                bool b = this.NodeInAngle(O, somebox[index1], somebox[index_r], somebox[index_p], somebox[i]);
                if (b == false)
                {
                    return false;
                    
                }

            }

            return true;

        }


        // Вычисление расстояния от точки до дуги (Формула 2.5)
        private double DistDuga(Node O, double R, Node M)
        {
            double c0 = O.X;
            double d0 = O.Y;
            double x = M.X;
            double y = M.Y;

            double d = Math.Abs(R-Math.Sqrt(Math.Pow(x-c0,2)+Math.Pow(y-d0,2)));
            return d;


        }
        

        // Вычисление максимального отклонения дуги окружности от ломанной
        private double Sigma(Node A, Node B, Node C, List<Node> somebox)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double s2 = C.X;
            double t2 = C.Y;

            bool f = false; // лежит ли основание перпендикуляра на отрезке
            double record = 0; // для записи максимального отклонения в списке перпендикуляров
            double dist = 0;
            // Получаем координаты центра окружности, из которой будет опущен перпендикуляр
            Node O = this.Circle(A,B,C);
            //Node O = this.Center(A, B, C);
            // Получаем радиус окружности
            double R = this.Circle_R(A,B,C);

            // Для каждого звена ломанной рассмотрим основание перпендикуляра и найдем максимальное отклонение от дуги
            for(int a=A.Index;a<=C.Index-1;a++) // не уверена 
            {
                f = this.Perpendicular(somebox[a],somebox[a+1],O);
                if(f== true)
                {
                    // Вычисляем координаты основания перпендикуляра
                    Node S = this.BasePerpendicular(somebox[a], somebox[a + 1], O);
                    dist = this.DistDuga(O,R,S);
                    if(dist> record)
                    {
                        record = dist;
                    }

                }
                else
                {
                    continue;
                }

            }

             
             for(int p=A.Index; p<=C.Index; p++)
            {
                dist = this.DistDuga(O,R,somebox[p]);
                if(dist>record)
                {
                    record = dist;
                }

            }

            return record;

        }

        // Вычисление кривизны дуги
        private double Curv(Node A, Node B, Node C)
        {
            double s0 = A.X;
            double t0 = A.Y;
            double s1 = B.X;
            double t1 = B.Y;
            double s2 = C.X;
            double t2 = C.Y;

            double R = this.Circle_R(A,B,C);
            double c = Math.Pow(s2 - s0, 2) + Math.Pow(t2 - t0, 2);

            double h = R - Math.Sqrt(R*R-c/4);
            double curv = 2 * h / Math.Sqrt(c);

            double v1 = (t1 - t0) * (s2 - s0) - (t2 - t0) * (s1-s0);
            double v2 = (s1 - s0) * (s2-s0);

            // if ((v1 * v2 > 0) || (s1 == s0 & (t1 - t0) * (s2 - s0) > 0) || (s2 == s0 & (t2 - t0) * (s1 - s0) < 0))
            if (v1 * v2 > 0 || ((s1==s0) & (t1-t0)*(s2-s0)>0) || ((s2==s0)& (t2-t0)*(s1-s0)<0))
            //if (v1 * v2 > 0 || ((s1 == s0) & v1> 0) ||((s2==s0)& v1 < 0))
            {
                curv = Math.Abs(curv);
            }
            else curv = -Math.Abs(curv);

            return curv;

        }

        

        // Считать данные из файла
        private void button1_Click(object sender, EventArgs e)
        {
            this.boxnode.Clear();
            this.OutputNode.Clear();
            this.boxcurv.Clear();
            this.textBox1.Clear();
            this.ReadNode();

        }

        
        private int Step2(int i, int j)
        {
            
           
            //MessageBox.Show(i.ToString());
            List<Node> listnode = this.boxnode.GetRange(i, this.boxnode.Count-i);
           // MessageBox.Show( "Количество узлов в шаге 2 " + listnode.Count.ToString());
            // Формируем множество X с учетом условия (*)
            // Первый и второй узлы всегда удовлетворяет условию (*)
            List<Node> X = new List<Node>();
             X.Add(listnode[0]);
             X.Add(listnode[1]);

            /*X.Add(boxnode[i]);
            X.Add(boxnode[i+1]);*/

            bool b = false;
            // Первые два узла добавили, значит начинаем с индекса 2
             /*for (int k = i+2; k < listnode.Count; k++)
             {
                 b = this.Uslovie(k, listnode);
                 if (b == true)
                 {
                     X.Add(listnode[k]);   
                 }

             }*/

            for (int k = 2; k < listnode.Count; k++)
            {
                
                b = this.Uslovie(k, listnode); //!!!! Неверно заданы данные (не исходный список, а список, начиная с i)

                if (b == true)
                {
                    X.Add(boxnode[k]);
                }

            }


            // Для каждого узла из X, начиная с p>=i+1
            // Формируем новый список
            List<Node> X1 = new List<Node>();
            double d = 0;
            /* for (int p =1 ; p < X.Count-1; p++)
             {
                 // Вычисляем расстояние от узлов до отрезка A_iA_p
                 for (int l = i+1; l < p; l++)
                 {
                     d = this.Dist(X[i], X[p], X[l]);
                     if (d <= ep)
                     {
                         X1.Add(X[p]);
                     }

                 }

             }*/

            bool m = false;
            foreach(Node Ap in X)
            {
                if (Ap.Index == i)
                    continue;
                if (Ap.Index == i+1)
                    { 
                        X1.Add(Ap); // Узел A_i+1 всегда попадает в X1, т.к. расстояние 0
                    continue;
                      }
                for(int l= i+1; l<=Ap.Index-1;l++)
                {
                    d = this.Dist(boxnode[i], boxnode[Ap.Index],boxnode[l]);
                    if (d <= ep)
                    {
                        m = true;
                        continue;
                    }
                    else
                    { 
                        m = false;
                        break;
                    }
                }

                if (m == true)
                {
                    X1.Add(Ap);
                }


            }
            

           // MessageBox.Show("Число узлов, расстояния не превышающие e  в шаге 2 : " + X1.Count.ToString()
              //  + " i= " + i.ToString());


            // Наибольший номер q
            Node n = X1[X1.Count-1];
            int q = n.Index;
            
            //q = this.Step2(i);
            /*if (q == this.boxnode.Count - 1)
            {
                this.Step3(q,j);
            }
            else
            {
                // Вызвать метод 4
                this.Step4(i, q,j);

            }*/
            return q;

        }

        // Шаг 3
     /*   private void Step3(int q,int j)
        {
            Node B_j = new Node(this.boxnode[q].X, this.boxnode[q].Y);
            this.OutputNode.Add(B_j);
            double curv_j;
            curv_j = 0;
            this.boxcurv.Add(curv_j);
            int i = q;
            j = j + 1;
            // Перейти к шагу 2
            this.Step2(i,j);
        
        }*/

        // Шаг 4
        private int Step4( ref int i, ref int q,  ref int j)
        {
            // Сформируем множество Y
            //Node[] Troika = new Node[3];// Массив троек узлов
            List<Node []> Y = new List<Node[]>();
            List<Node[]> Z = new List<Node[]>();
           // List<Node> listnode = this.boxnode.GetRange(i, this.boxnode.Count - i);
            bool b = false;
            int ind = -1; // индекс для записи тройки, имеющей наибольшее p
            int pglob = -1; // наибольшее p
            for (int p= q+1; p< this.boxnode. Count; p++)
            {
                for(int r= i+1; r<p;r++)
                {
                    b = this.TrineNodex(i,r,p,boxnode);

                    if (b == true)
                    {
                        Node[] Troika = new Node[3];// Массив троек узлов
                        Troika[0] = this.boxnode[i];
                        Troika[1] = this.boxnode[r];
                        Troika[2] = this.boxnode[p];
                        Y.Add(Troika);

                    }
                    
                }

            }

            double s = 0; // Отклонение дуги от ламаной
           


            if(Y.Count == 0)
            {
                // Перейти к шагу 3!!!!!!
                return 3;
            }

             else
            {
                foreach(Node [] tr in Y)
                {
                    //s = this.Sigma(tr[0], tr[1], tr[2], this.boxnode.GetRange(i, tr[2].Index - i));
                    s = this.Sigma(tr[0], tr[1], tr[2], this.boxnode);
                   // MessageBox.Show("Отклонение дуги от ломанной  " + s.ToString() );

                    if (s <= ep)
                    {
                        Z.Add(tr);
                        if (tr[2].Index >= pglob)
                        {
                            pglob = tr[2].Index;
                            ind = Z.Count - 1;// Индекс тройки в списке Z т.к. эта тройка добавилась в конец
                        }

                    }
                    else
                        continue;

                }

                if (Z.Count == 0)
                {
                    //this.Step3(q,j);
                    // Перейти к шагу 3
                    return 3;

                }

                 else
                {
                    // Берем тройку с максимальным p
                    Node[] duga = Z[ind];
                    // Вычисляем кривизну дуги
                    double curvdug = this.Curv(duga[0], duga[1], duga[2]);
                    Node B_j = new Node(duga[2].X, duga[2].Y);
                    this.OutputNode.Add(B_j);
                    this.boxcurv.Add(curvdug);
                    i = duga[2].Index;
                    j = j + 1;
                    //this.Step2(i,j);
                    return 2;

                }
                

            }



        }

        // Шаг 5
        private void Step5(int j)
        {
            // m=j
            if(this.OutputNode.Count-1!=j || this.boxcurv.Count -1 !=j-1)
            {
                MessageBox.Show("Количество узлов не совпадает");
                    return;
            }
            else
            {
                // Вызов шага 5

               // MessageBox.Show(" Количество узлов: " + this.OutputNode.Count.ToString() +
                //   "\r\n" + this.boxcurv.Count.ToString());
                string s = "";
                s += this.OutputNode[0].X.ToString() + " " + this.OutputNode[0].Y.ToString()
                   + " " + this.boxcurv[0].ToString();
                for (int h = 1; h < OutputNode.Count-1; h++) // до -1 т.к. в коробке кривиз на 1 меньше
                {
                   
                   /* for (int m = 1; m < boxcurv.Count; m++)
                    {

                       s += " \r\n" + OutputNode[h].X.ToString() + "  " + OutputNode[h].Y.ToString()
                                + "  " + boxcurv[m].ToString();
                       // break;
                    }*/

                    s += " \r\n" + OutputNode[h].X.ToString() + " " + OutputNode[h].Y.ToString()
                                + " " + boxcurv[h].ToString();

                }

                s += " \r\n" + OutputNode[OutputNode.Count - 1].X.ToString() + " " + OutputNode[OutputNode.Count - 1].Y.ToString();

                this.textBox1.Text = s;

            }


        }
         
        // Шаг 1
        private void Step1()
        {
            Node B1 = this.boxnode[0];
            this.OutputNode.Add(B1);
            int i = 0;
            int j = 0;

            this.Step2(i, j);


        }

       //Вспомогательный метод для нахождения ближайшего звена
       /* private double NearestLink() // метод возвращает расстояние до ближайшего звена
        {



        }*/


        // Запуск алгоритма аппроксимации
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Update();
            if (boxnode == null || boxnode.Count ==0)
            {
                MessageBox.Show(" Список исходных узлов пуст!");
                return;
                   
            }
            string line = textBox2.Text;
            int ind1 = line.IndexOf(".");
            if(ind1 ==-1 )
            {
                ep = Convert.ToDouble(line);
            }
             else
            {
                 line = line.Replace(".", ",");
                //MessageBox.Show(line);
                ep = Convert.ToDouble(line);
            }

            if (ep == 0)
            {
                MessageBox.Show("Недопустимый шаг аппроксимации!");
                return;
            }

            else

            //this.Step1();
            {
                
                this.OutputNode.Clear();
                this.boxcurv.Clear();
                Node B1 = this.boxnode[0];
                this.OutputNode.Add(B1);
                int i = 0;
                int j = 0;
                double curv_j;
                int q = -1;
                while (i != this.boxnode.Count -1)
                 {
                      q=  this.Step2(i, j);
                   // MessageBox.Show( " q=  " + q.ToString() + "  " + "i = " + i.ToString());
                      if (q == this.boxnode.Count -1)
                      {
                          // Шаг 3
                          Node B_j = new Node(this.boxnode[q].X, this.boxnode[q].Y);
                        //this.OutputNode.Add(B_j);
                        this.OutputNode.Insert(j+1, B_j);
                         
                          curv_j = 0;
                        //this.boxcurv.Add(curv_j);
                        this.boxcurv.Insert(j,curv_j);
                          i = q;
                          j = j + 1;
                          continue;
                         

                      }
                      else
                      {
                             // Вызвать шаг 4
                         int flag =  this.Step4(ref i, ref q, ref j);
                           
                          if (flag == 3)
                          {
                              // Шаг 3
                              Node B_j = new Node(this.boxnode[q].X, this.boxnode[q].Y);
                            //this.OutputNode.Add(B_j);
                             this.OutputNode.Insert(j+1,B_j);
                              curv_j = 0;
                            //this.boxcurv.Add(curv_j);
                              this.boxcurv.Insert(j,curv_j);
                              i = q;
                              j = j + 1;
                              continue;
                          }

                          else 
                          {
                            continue;

                          }

                      }

                 }
                   

                if (i == this.boxnode.Count -1)
                {
                    this.Step5(j);
                }


            }



            MessageBox.Show("Количество выходных узлов  " + this.OutputNode.Count.ToString());

        }

        // Левый метод поиска центра окружности
        //http://algolist.manual.ru/maths/geom/equation/circle.php
        private Node Center(Node P1, Node P2, Node P3)
        {
            double x1 = P1.X;
            double y1 = P1.Y;
            double x2 = P2.X;
            double y2 = P2.Y;
            double x3 = P3.X;
            double y3 = P3.Y;

            double ma = (y2-y1) / (x2-x1);
            double mb = (y3-y2) / (x3-x2);

            double x = (ma * mb * (y1 - y3) + mb*(x1 + x2) - ma*(x2 + x3)) / (2 * (mb - ma));
            double y = -(1 / ma) * (x - (x1 + x2) / 2) + (y1+y2) / 2;
            Node O = new Node(x,y);
            return O;

        }

        // Тестирование
        private void button4_Click(object sender, EventArgs e)
        {
            // Проверка вычисления координат центра окружности дуги
            /*Node A = new Node(0,1);
            double X = Math.Round(Math.Sqrt(2) / 2,2);
            double Y = Math.Round(Math.Sqrt(2) / 2,2);
            Node B = new Node(X, Y);
            Node C = new Node(1,0);*/
            Node A = this.boxnode[0];
            Node B = this.boxnode[2];
            Node C = this.boxnode[4];

            /* bool b = this.TrineNodex(0, 1, 4, this.boxnode);
             MessageBox.Show(b.ToString());*/

            /* Node O = this.Circle(A,B,C);
             double R = this.Circle_R(A,B,C);
             bool perp = this.Perpendicular(A,B,O);

             MessageBox.Show("X =  " + O.X + "Y = " + O.Y 
                 + "R =  " + R);
             double S = Math.Sqrt(2) / 2;*/
            // MessageBox.Show("S =  " + S);
            //   + "R =  " + R);

            Node O = this.Center(A,B,C);
            MessageBox.Show("X=  " + O.X + "   Y=  " + O.Y);
            Node O1 = this.Circle(A,B,C);
            MessageBox.Show("X=  " + O1.X + "   Y=  " + O1.Y);

        }


        // Записать результаты в файл
        private void button3_Click(object sender, EventArgs e)
        {
            if(this.textBox1.Text ==" ")
            {
                MessageBox.Show("Выходная строка пуста!");
                return;
            }
            string output = "";
            //string newname = ""; // для записи нового имени файла
            try
            {

                if(this.path == " ")
                {
                    string path = Environment.CurrentDirectory;// Путь к рабочей папке
                }

                int index = filename.IndexOf("coord");
                filename = filename.Substring(0, index-1);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path + @"\" + filename+ @"_Result.txt"  , false,System.Text.Encoding.GetEncoding(1251));
                output += this.textBox1.Text;
                sw.Write(output);
                sw.Close();
                MessageBox.Show("Результат записан в файл" + path + @"\" + filename + @"_Result.txt");

            }

            catch
            {
                MessageBox.Show("Ошибка записи в файл");
            }


        }
    }
}
