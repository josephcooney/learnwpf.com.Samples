using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ZMQ;
using Exception = System.Exception;

namespace InProcess0mq
{
    public partial class MainWindow : Window
    {
        private InProcContext context;
        private PollItem[] pollItems;
        private DispatcherTimer pollingTimer;

        public MainWindow()
        {
            InitializeComponent();
            context = new InProcContext();
            Closing += MainWindowClosing;
            Loaded += MainWindowLoaded;
        }

        private void StartServer()
        {
            var socket = context.InProcSocket(SocketType.PUB);            
            socket.Bind("inproc://foobar");
            socket.Linger = 0;
            var server = new SomeKindOfServer(socket);
            var t = new Thread(server.Start) {IsBackground = true};
            t.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            int countOfItems = 0;
            do
            {
                countOfItems = context.Poll(pollItems, 0);
            } while (countOfItems == 1);

            Console.WriteLine("Polling completed");
        }

        private void InitializeMessageReceival()
        {
            var client = context.InProcSocket(SocketType.SUB);
            client.Subscribe("date:", Encoding.Unicode);
            client.Connect("inproc://foobar");
            client.Linger = 0;
            var item = client.CreatePollItem(IOMultiPlex.POLLIN);
            item.PollInHandler += client_PollInHandler;
            pollItems = new[] { item };
        }


        void client_PollInHandler(Socket socket, IOMultiPlex revents)
        {
            string postedMessage = socket.Recv(Encoding.Unicode);
            someText.Text = postedMessage;
            Console.WriteLine("Received message : {0}", postedMessage);
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            StartServer();
            InitializeMessageReceival();
            pollingTimer = new DispatcherTimer(DispatcherPriority.Normal);
            pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            pollingTimer.Tick += dt_Tick;
            pollingTimer.IsEnabled = true;
            pollingTimer.Start();
            
        }

        void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pollingTimer.Stop();
            context.Dispose();
        }
    }

    public class InProcContext : Context
    {
        private List<Socket> sockets = new List<Socket>();

        public Socket InProcSocket(SocketType socketType)
        {
            var socket = Socket(socketType);
            lock (sockets)
            {
                sockets.Add(socket);                
            }
            return socket;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (sockets)
                {
                    foreach (var socket in sockets)
                    {
                        lock (socket)
                        {
                            socket.Dispose();
                        }
                    }
                    sockets = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    public class SomeKindOfServer
    {
        private readonly Socket socket;

        public SomeKindOfServer(Socket socket)
        {
            this.socket = socket;
        }

        public void Start()
        {
            var rnd = new Random(System.DateTime.Now.Millisecond);
            try
            {
                while (true)
                {
                    // Do Some 'work'
                    var sleepTime = rnd.Next(100, 200); 
                    Thread.Sleep(sleepTime);
                    var message = string.Format("date: I slept for {0} and now it is {1} ", sleepTime, DateTime.Now.ToLongTimeString());
                    Console.WriteLine(message);

                    // Send reply back to client
                    lock (socket)
                    {
                        socket.Send(message, Encoding.Unicode);                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown...SomeKindOfServer is shutting down");
            }
        }
    }
}
