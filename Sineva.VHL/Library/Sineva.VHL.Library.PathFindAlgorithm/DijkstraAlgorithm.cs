
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sineva.VHL.Library.PathFindAlgorithm
{
    public class DijkstraAlgorithm
    {
        #region Fields
        private List<LinkItem> _Links = new List<LinkItem>();
        #endregion

        #region Properties
        #endregion

        #region Events
        #endregion

        #region Constructors
        private DijkstraAlgorithm()
        {
        }
        #endregion

        #region Methods - Dijkstra
        static public int[] Dijkstra(PathFindMethod findMethod, bool applyWeight, double[,] matrixArray, int sourceIndex, int destinationIndex, out double totalCost)
        {
            try
            {
                int length = matrixArray.GetLength(0);
                double[] costData = new double[length];
                bool[] blockData = new bool[length];
                int[] parentInfo = new int[length];
                for (int i = 0; i < length; i++)
                {
                    costData[i] = Constants.INFINITE;
                    blockData[i] = false;
                    parentInfo[i] = -1;
                }

                costData[sourceIndex] = 0;

                int current = sourceIndex;

                //foreach (int from in matrixArray.Keys)
                for (int from = 0; from < length; from++)
                {
                    double min = Constants.INFINITE;
                    int u = -1;

                    //foreach (int node in matrixArray.Keys)
                    for (int node = 0; node < length; node++)
                    {
                        if (blockData[node] == false && costData[node] <= min)
                        {
                            min = costData[node];
                            u = node;
                        }
                    }

                    blockData[u] = true;

                    int length2 = matrixArray.GetLength(1);
                    for (int to = 0; to < length2; to++)
                    {
                        if (!blockData[to] && matrixArray[u, to] > 0 && costData[u] + matrixArray[u, to] < costData[to])
                        {
                            parentInfo[to] = u;
                            costData[to] = costData[u] + matrixArray[u, to];
                        }
                    }
                }
                totalCost = costData[destinationIndex];
                return parentInfo;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                totalCost = Constants.INFINITE;
                return new int[0];
            }
        }
        static public int[] Dijkstra(double[,] matrixArray, int sourceIndex)
        {
            try
            {
                int length = matrixArray.GetLength(0);
                double[] costData = new double[length];
                bool[] blockData = new bool[length];
                int[] parentInfo = new int[length];
                for (int i = 0; i < length; i++)
                {
                    costData[i] = Constants.INFINITE;
                    blockData[i] = false;
                    parentInfo[i] = -1;
                }

                costData[sourceIndex] = 0;

                int current = sourceIndex;

                //foreach (int from in matrixArray.Keys)
                for (int from = 0; from < length; from++)
                {
                    double min = Constants.INFINITE;
                    int u = -1;

                    //foreach (int node in matrixArray.Keys)
                    for (int node = 0; node < length; node++)
                    {
                        if (blockData[node] == false && costData[node] <= min)
                        {
                            min = costData[node];
                            u = node;
                        }
                    }

                    blockData[u] = true;

                    int length2 = matrixArray.GetLength(1);
                    for (int to = 0; to < length2; to++)
                    {
                        if (!blockData[to] && costData[u] + matrixArray[u, to] < costData[to])
                        {
                            parentInfo[to] = u;
                            costData[to] = costData[u] + matrixArray[u, to];
                        }
                    }
                }
                return parentInfo;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return new int[0];
            }
        }

        static public List<int> MakePathByDijkstra(double[,] mapInfo, int sourceIndex, int destinationIndex, int[] parentArray, out double totalCost)
        {
            try
            {
                List<int> shortestPath = new List<int>();
                int current = destinationIndex;
                totalCost = 0;

                while (current != sourceIndex)
                {
                    shortestPath.Add(current);

                    int old = current;
                    current = parentArray[current];

                    if (current == -1)
                    {
                        return new List<int>();
                    }

                    try
                    {
                        totalCost += mapInfo[current, old];
                    }
                    catch (Exception ex)
                    {
                        System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                        ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                        totalCost = 0;
                        return new List<int>();
                    }

                    if (current < 0)
                    {
                        shortestPath.Clear();
                        return shortestPath;
                    }

                    Thread.Sleep(1);
                }

                if (shortestPath.Count != 0)
                {
                    shortestPath.Add(sourceIndex);
                    shortestPath.Reverse();
                }

                return shortestPath;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                totalCost = 0;
                return new List<int>();
            }
        }

        static public List<int> MakePathByDijkstra(int sourceIndex, int destinationIndex, int[] parentArray)
        {
            #region "Code"
            try
            {
                List<int> shortestPath = new List<int>();
                int current = destinationIndex;
                int old = 0;

                while (current != sourceIndex)
                {
                    shortestPath.Add(current);

                    old = current;
                    current = parentArray[current];

                    if (current == -1)
                    {
                        return new List<int>();
                    }

                    if (current < 0)
                    {
                        shortestPath.Clear();
                        return shortestPath;
                    }
                    Thread.Sleep(1);
                }

                if (shortestPath.Count != 0)
                {
                    shortestPath.Add(sourceIndex);
                    shortestPath.Reverse();
                }

                return shortestPath;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return new List<int>();
            }
            #endregion
        }

        #endregion
    }
}
