using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Winform_Bai_Toan_Thap_Ha_Noi
{
    public partial class Form1 : Form
    {

        // Tự cài lại Stack không dùng Stack có sẵn của .NET để dễ dàng thao tác với các cột trong bài toàn Tháp Hà Nội, với các phương thức Push, Pop, Peek, GetAll và Clear để quản lý các đĩa trên mỗi cột một cách hiệu quả và dễ hiểu
        class MyStack // sử dụng mảng để lưu trữ các đĩa trên mỗi cột, với biến top để theo dõi định vị của đĩa ở đỉnh ngăn xếp và biến capacity để kiểm soát số lượng đĩa có thể được thêm vào ngăn xếp, giúp tránh tràn ngăn xếp khi thêm quá nhiều đĩa và đảm bảo tuân thủ luật chơi Tháp Hà Nội khi di chuyển các đĩa
        {
            private int[] arr;// mảng để lưu trữ các đĩa trên cột, với mỗi phần tử đại diện cho kích thước của một đĩa, trong đó giá trị lớn hơn biểu thị một đĩa lớn hơn
            private int top;// biến để theo dõi vị trí của đĩa ở đỉnh của ngăn xếp, với giá trị -1 biểu thị rằng ngăn xếp đang rỗng
            private int capacity;// biến để lưu trữ sức chứa tối đa của ngăn xếp, giúp kiểm soát số lượng đĩa có thể được thêm vào ngăn xếp và tránh tràn ngăn xếp khi thêm quá nhiều đĩa

            public MyStack(int size = 10)
            {
                capacity = size;
                arr = new int[capacity];
                top = -1;
            }

            public void Push(int x)
            {
                if (top == capacity - 1)
                    throw new Exception("Stack đầy rồi!");

                if (top != -1 && arr[top] < x)
                    throw new Exception("Sai luật Hanoi rồi!");

                arr[++top] = x;
            }

            public int Pop()
            {
                if (top == -1) return 0;
                return arr[top--];
            }

            public int Peek()
            {
                if (top == -1) return 0;
                return arr[top];
            }

            public int[] GetAll()// phương thức để lấy tất cả các đĩa hiện có trên ngăn xếp dưới dạng một mảng, với kích thước của mảng bằng số lượng đĩa hiện có (top + 1), giúp dễ dàng truy cập và hiển thị các đĩa trên giao diện người dùng của trò chơi Tháp Hà Nội
            {
                int[] result = new int[top + 1];

                for (int i = 0; i <= top; i++)
                {
                    result[i] = arr[i];
                }

                return result;
            }

            public void Clear()//xoa tất cả các đĩa trên ngăn xếp bằng cách đặt biến top về -1, biểu thị rằng ngăn xếp đang rỗng và không chứa bất kỳ đĩa nào, giúp chuẩn bị ngăn xếp cho một trò chơi mới hoặc khi người dùng muốn đặt lại trò chơi về trạng thái ban đầu
            {
                top = -1;
            }
        }
        private void Form1_Load(object sender, EventArgs e)// sự kiện tải của form, có thể được sử dụng để thực hiện các thiết lập ban đầu hoặc khởi tạo trạng thái của trò chơi khi form được hiển thị lần đầu tiên
        {

        }

        // tạo ba ngăn xếp A, B và C để biểu diễn ba cột trong bài toàn Tháp Hà Nội, cùng với một danh sách steps để lưu trữ các bước di chuyển đĩa, một biến currentStep để theo dõi bước hiện tại, và một Timer để tự động thực hiện các bước di chuyển sau khi người dùng bấm nút Start
        MyStack A = new MyStack(10);
        MyStack B = new MyStack(10);
        MyStack C = new MyStack(10);

        List<(char from, char to)> steps = new List<(char, char)>();// danh sách để lưu trữ các bước di chuyển đĩa, mỗi bước được biểu diễn dưới dạng một tuple chứa ký tự đại diện cho cột xuất phát và cột đích
        int currentStep = 0;

        Timer timer;// Timer để tự động thực hiện các bước di chuyển sau khi người dùng bấm nút Start, với một khoảng thời gian xác định giữa mỗi bước

        Panel pA, pB, pC;// Các panel để hiển thị ba cột trong trò chơi Tháp Hà Nội, mỗi panel sẽ được sử dụng để vẽ các đĩa tương ứng với cột đó
        Button btnStart, btnStep, btnReset;// Các nút để bắt đầu trò chơi, thực hiện bước tiếp theo và đặt lại trò chơi về trạng thái ban đầu
        NumericUpDown nud;// NumericUpDown để cho phép người dùng chọn số lượng đĩa trong trò chơi, với giá trị tối thiểu là 1 và tối đa là 8
        Label lbl;// Label để hiển thị số bước đã thực hiện trong trò chơi, cập nhật mỗi khi một bước di chuyển được thực hiện

        int diskCount = 3;

        public Form1()// constructor của form, nơi khởi tạo các thành phần giao diện người dùng, thiết lập trò chơi với số lượng đĩa mặc định là 3, và cấu hình timer để thực hiện các bước di chuyển tự động khi bắt đầu trò chơi
        {
            InitializeComponent();
            InitUI();
            InitGame(3);

            timer = new Timer();
            timer.Interval = 400;
            timer.Tick += Timer_Tick;
        }
        void InitUI()// phương thức để thiết lập giao diện người dùng của form, bao gồm việc tạo và cấu hình các panel, nút, NumericUpDown và Label, cũng như gán các sự kiện click cho các nút để điều khiển trò chơi
        {
            this.Text = "Winform giao diện basic Tháp Hà Nội - Team 2 biểu diễn cực nghệ luôn thưa thầy Thành và các ace IT001  ";
            this.Size = new Size(900, 550);
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.DoubleBuffered = true;// Kích hoạt DoubleBuffered để giảm hiện tượng nhấp nháy khi vẽ lại giao diện người dùng, đặc biệt là khi di chuyển các đĩa trên các cột trong trò chơi Tháp Hà Nội

            pA = CreatePanel(150);// Tạo ba panel pA, pB và pC để hiển thị các cột trong trò chơi Tháp Hà Nội, với vị trí ngang được xác định trước (150, 400 và 650) để đảm bảo rằng các cột được đặt đúng cách trên form
            pB = CreatePanel(400);
            pC = CreatePanel(650);

            btnStart = CreateButton("❤️ Start", 150);// Tạo nút Start với văn bản "❤️ Start" và vị trí ngang là 150, sử dụng phương thức CreateButton để đảm bảo kiểu dáng nhất quán cho các nút trong giao diện người dùng của trò chơi Tháp Hà Nội
            btnStep = CreateButton("💙 Step", 300);
            btnReset = CreateButton("💛 Reset", 450);

            pA.BackColor = Color.Transparent;// đặt màu nền của các panel pA, pB và pC thành trong suốt để cho phép hiển thị các đĩa và cột một cách rõ ràng trên nền của form, tạo hiệu ứng trực quan tốt hơn cho trò chơi Tháp Hà Nội
            pB.BackColor = Color.Transparent;
            pC.BackColor = Color.Transparent;
            nud = new NumericUpDown()// Tạo một NumericUpDown để cho phép người dùng chọn số lượng đĩa trong trò chơi Tháp Hà Nội, với giá trị tối thiểu là 1, tối đa là 8, và giá trị mặc định là 3, cùng với vị trí ngang là 650 để đặt nó gần nút Start và các cột
            {
                Minimum = 1,
                Maximum = 8,
                Value = 3,
                Location = new Point(650, 420),
                Width = 60
            };

            lbl = new Label()// Tạo một Label để hiển thị số bước đã thực hiện trong trò chơi Tháp Hà Nội, với văn bản mặc định là "Bước: 0", màu chữ trắng, font chữ Segoe UI kích thước 12 và in đậm, cùng với vị trí ngang là 750 để đặt nó gần nút Step và nút Reset
            {
                Text = "Bước: 0",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(750, 420),
                AutoSize = true
            };

            this.Controls.AddRange(new Control[]
            {
        pA,pB,pC,btnStart,btnStep,btnReset,nud,lbl
            });

            btnStart.Click += (s, e) =>// Gán sự kiện click cho nút Start để khi người dùng bấm vào nút này, trò chơi sẽ được đặt lại về trạng thái ban đầu và bắt đầu thực hiện các bước di chuyển đĩa một cách tự động thông qua timer
            {
                ResetAll();
                Hanoi(diskCount, 'A', 'B', 'C');
                timer.Start();
            };

            btnStep.Click += (s, e) =>// Gán sự kiện click cho nút Step để khi người dùng bấm vào nút này, trò chơi sẽ thực hiện bước di chuyển tiếp theo trong danh sách steps một cách thủ công, cho phép người dùng kiểm soát quá trình di chuyển đĩa từng bước một
            {
                timer.Stop();
                if (steps.Count == 0)
                    Hanoi(diskCount, 'A', 'B', 'C');

                RunStep();
            };

            btnReset.Click += (s, e) =>// Gán sự kiện click cho nút Reset để khi người dùng bấm vào nút này, trò chơi sẽ được đặt lại về trạng thái ban đầu, bao gồm việc xóa tất cả các bước di chuyển đã lưu, đặt lại biến currentStep về 0, cập nhật số lượng đĩa dựa trên giá trị của NumericUpDown, cập nhật label để hiển thị bước hiện tại là 0, và gọi InitGame để khởi tạo lại trò chơi với số lượng đĩa mới
            {
                timer.Stop();
                ResetAll();
            };
        }
        Button CreateButton(string text, int x)// tạo phương thức để tạo một nút với văn bản, vị trí và kiểu dáng được xác định trước, giúp đơn giản hóa việc tạo các nút Start, Step và Reset trong giao diện người dùng của trò chơi Tháp Hà Nội
        {
            return new Button()// Tạo một Button với văn bản được chỉ định, vị trí ngang được xác định bởi tham số x, kích thước cố định là 120x40, màu nền tối, màu chữ trắng, kiểu dáng phẳng và font chữ Segoe UI kích thước 10 và in đậm, tạo nên một giao diện nhất quán và dễ nhìn cho các nút trong trò chơi Tháp Hà Nội
            {
                Text = text,
                Location = new Point(x, 420),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        Panel CreatePanel(int x)// tạo phương thức để tạo một panel với vị trí và kiểu dáng được xác định trước, giúp đơn giản hóa việc tạo các panel pA, pB và pC trong giao diện người dùng của trò chơi Tháp Hà Nội, mỗi panel sẽ được sử dụng để hiển thị các đĩa trên cột tương ứng
        {
            return new Panel()// Tạo một Panel với vị trí ngang được xác định bởi tham số x, kích thước cố định là 120x250, và màu nền tối, tạo nên một giao diện nhất quán và dễ nhìn cho các cột trong trò chơi Tháp Hà Nội
            {
                Location = new Point(x, 100),
                Size = new Size(120, 250),
                BackColor = Color.FromArgb(30, 30, 50)
            };
        }



        void InitGame(int n)// phương thức để khởi tạo trạng thái của trò chơi với n đĩa, bằng cách xóa tất cả các đĩa hiện có trên ba cột và sau đó thêm các đĩa mới vào cột A theo thứ tự từ lớn đến nhỏ, sau đó vẽ lại giao diện để hiển thị trạng thái mới của trò chơi
        {
            A = new MyStack(diskCount);
            B = new MyStack(diskCount);
            C = new MyStack(diskCount);
            A.Clear(); B.Clear(); C.Clear();

            for (int i = n; i >= 1; i--)
                A.Push(i);

            DrawAll();
        }


        void ResetAll()// phương thức để đặt lại trò chơi về trạng thái ban đầu, bằng cách xóa tất cả các bước di chuyển đã lưu, đặt lại biến currentStep về 0, cập nhật số lượng đĩa dựa trên giá trị của NumericUpDown, cập nhật label để hiển thị bước hiện tại là 0, và gọi InitGame để khởi tạo lại trò chơi với số lượng đĩa mới
        {
            steps.Clear();
            currentStep = 0;
            diskCount = (int)nud.Value;
            lbl.Text = "Bước: 0";
            InitGame(diskCount);
        }


        void Timer_Tick(object sender, EventArgs e)// sự kiện được gọi mỗi khi timer tick, kiểm tra nếu currentStep nhỏ hơn tổng số bước trong danh sách steps, nếu có thì gọi RunStep để thực hiện bước tiếp theo, nếu không thì dừng timer vì đã hoàn thành tất cả các bước di chuyển
        {
            if (currentStep < steps.Count)
                RunStep();
            else
                timer.Stop();
        }

        void RunStep()// phương thức để thực hiện bước tiếp theo trong danh sách steps, cập nhật trạng thái trò chơi và nhãn để phản ánh bước hiện tại
        {
            var s = steps[currentStep];
            Move(s.from, s.to);
            currentStep++;
            lbl.Text = "Bước: " + currentStep;
        }


        MyStack Get(char c)// phương thức để lấy ngăn xếp tương ứng với cột được chỉ định bởi ký tự c ('A', 'B' hoặc 'C'), giúp đơn giản hóa việc truy cập và thao tác với các ngăn xếp đại diện cho ba cột trong trò chơi Tháp Hà Nội
        {
            if (c == 'A') return A;
            if (c == 'B') return B;
            return C;
        }

        void Move(char from, char to)// phương thức để di chuyển một đĩa từ cột "from" sang cột "to", bằng cách lấy ngăn xếp tương ứng cho mỗi cột, lấy đĩa ở đỉnh của ngăn xếp "from" và thêm nó vào ngăn xếp "to", sau đó gọi DrawAll để cập nhật giao diện người dùng và hiển thị trạng thái mới của trò chơi
        {
            MyStack s = Get(from);
            MyStack t = Get(to);

            int disk = s.Pop();
            t.Push(disk);

            DrawAll();
        }

        void Hanoi(int n, char A, char B, char C)// phương thức đệ quy để giải quyết bài toán Tháp Hà Nội, với n là số lượng đĩa, A là cột xuất phát, B là cột trung gian và C là cột đích; phương thức này sẽ thêm các bước di chuyển cần thiết vào danh sách steps để sau này có thể thực hiện các bước di chuyển một cách tự động hoặc theo từng bước khi người dùng yêu cầu
        {
            if (n == 1)
            {
                steps.Add((A, C));
                return;
            }

            Hanoi(n - 1, A, C, B);
            steps.Add((A, C));
            Hanoi(n - 1, B, A, C);
        }

        void DrawAll()// phương thức để vẽ lại tất cả các đĩa trên ba cột bằng cách gọi phương thức Draw cho mỗi panel tương ứng với ngăn xếp của mỗi cột, đảm bảo rằng giao diện người dùng luôn phản ánh trạng thái hiện tại của trò chơi sau mỗi bước di chuyển
        {
            Draw(pA, A);
            Draw(pB, B);
            Draw(pC, C);
        }
        protected override void OnPaint(PaintEventArgs e)// phương thức được ghi đè để vẽ các thành phần tĩnh của trò chơi, như các cột, bằng cách gọi phương thức DrawRod cho mỗi vị trí cột trên form, đảm bảo rằng các cột luôn được hiển thị đúng cách bất kể trạng thái của trò chơi
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            DrawRod(g, 210);
            DrawRod(g, 460);
            DrawRod(g, 710);
        }
        void DrawRod(Graphics g, int x)// phương thức để vẽ một cột trên form tại vị trí ngang được chỉ định, sử dụng bút màu xám để tạo một đường thẳng đứng đại diện cho cột
        {
            using (Pen pen = new Pen(Color.Gray, 6))
            {
                g.DrawLine(pen, x, 120, x, 350);
            }
        }

        void Draw(Panel panel, MyStack stack)// phương thức để vẽ các đĩa trên một panel tương ứng với ngăn xếp của một cột, bằng cách xóa tất cả các điều khiển hiện có trên panel và sau đó tạo và thêm các panel nhỏ đại diện cho mỗi đĩa trong ngăn xếp, với kích thước và vị trí được tính toán dựa trên kích thước của đĩa và số lượng đĩa hiện có trên cột
        {
            panel.Controls.Clear();

            int y = panel.Height - 25;

            foreach (var disk in stack.GetAll())// duyệt qua tất cả các đĩa trong ngăn xếp và tạo một panel nhỏ cho mỗi đĩa, với chiều rộng được tính dựa trên kích thước của đĩa (disk * 22) và chiều cao cố định là 18, sau đó đặt vị trí của panel sao cho nó được căn giữa trên panel chính và có khoảng cách đều nhau giữa các đĩa
            {
                Panel d = new Panel();
                d.Height = 18;
                d.Width = disk * 22;

                d.Left = (panel.Width - d.Width) / 2;
                d.Top = y;

                d.Paint += (s, e) =>
                {
                    Graphics g = e.Graphics;
                    Rectangle rect = new Rectangle(0, 0, d.Width - 1, d.Height - 1);

                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rect,
                        Color.FromArgb(0, 200, 255),
                        Color.FromArgb(0, 80, 200),
                        LinearGradientMode.Horizontal))// Sử dụng LinearGradientBrush để tạo hiệu ứng màu gradient cho các đĩa, với màu bắt đầu là một màu xanh sáng (Color.FromArgb(0, 200, 255)) và kết thúc là một màu xanh đậm hơn (Color.FromArgb(0, 80, 200)), tạo nên một hiệu ứng trực quan hấp dẫn cho các đĩa trong trò chơi Tháp Hà Nội
                    {
                        g.FillRectangle(brush, rect);
                    }

                    g.DrawRectangle(Pens.White, rect);
                };

                y -= 22;
                panel.Controls.Add(d);
            }
        }
    }
}
