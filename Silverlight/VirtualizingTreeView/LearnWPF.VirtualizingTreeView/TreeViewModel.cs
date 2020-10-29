using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LearnWPF.VirtualizingTreeView
{
    public class NodeViewModel<T> : INotifyPropertyChanged
    {
        private TreeViewModel<T> parentViewModel;
        private readonly T model;
        private readonly int indentation;
        private readonly Func<T, IEnumerable<T>> getChildrenFunction;
        bool isExpanded = false;


        public NodeViewModel(TreeViewModel<T> parentModel, T model, int indentation, Func<T, IEnumerable<T>> getChildrenFunction)
        {
            this.parentViewModel = parentModel;
            this.model = model;
            this.indentation = indentation;
            this.getChildrenFunction = getChildrenFunction;
        }

        public int Indentation { get { return indentation; } }
        public double IndentationDistance { get { return indentation * 15; } }
        public T Model { get { return model; } }

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                    RaisePropertyChanged("ArrowAngle");

                    parentViewModel.ToggleExpanded(this);
                }
            }
        }

        public double ArrowAngle
        {
            get
            {
                if (isExpanded)
                {
                    return 225;
                }
                return 135;
            }
        }

        public Visibility ExpanderVisibility
        {
            get
            {
                if (this.Children != null && this.Children.Count() > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public IEnumerable<T> Children
        {
            get { return getChildrenFunction(this.model); }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TreeViewModel<T>
    {
        private readonly Func<T, IEnumerable<T>> getChildFunction;

        public TreeViewModel(T root, Func<T, IEnumerable<T>> getChildFunction)
        {
            this.getChildFunction = getChildFunction;
            Nodes = new ObservableCollection<NodeViewModel<T>>();
            Nodes.Add(new NodeViewModel<T>(this, root, 0, getChildFunction));
        }

        public void ToggleExpanded(NodeViewModel<T> nodeViewModel)
        {
            var index = Nodes.IndexOf(nodeViewModel);
            if (nodeViewModel.IsExpanded && nodeViewModel.Children != null)
            {
                foreach (var child in nodeViewModel.Children)
                {
                    index++;
                    Nodes.Insert(index, new NodeViewModel<T>(this, child, nodeViewModel.Indentation + 1, getChildFunction));
                }
            }

            if (!nodeViewModel.IsExpanded)
            {
                while (Nodes.Count > index && nodeViewModel.Indentation < Nodes[index + 1].Indentation)
                {
                    Nodes.RemoveAt(index + 1);
                }
            }
        }

        public ObservableCollection<NodeViewModel<T>> Nodes
        {
            get;
            private set;
        }
    }
}
