using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using Sineva.VHL.Library.PathFindAlgorithm;
using Sineva.VHL.Data.DbAdapter;

namespace Sineva.VHL.Data.PathFinder
{
    public class PathFinderMain : Singleton<PathFinderMain>
    {
        #region Fields
        private double[,] _DistanceMap;
        private double[,] _TimeMap;
        private Dictionary<int, int> _WeightMap = new Dictionary<int, int>();
        #endregion

        #region Properties
        #endregion

        #region Events
        #endregion

        #region Constructors
        private PathFinderMain()
        {
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            try
            {
                bool result = true;

                MakeMatrix();

                foreach (DataItem_Link link in DatabaseHandler.Instance.DictionaryLinkDataList.Values)
                {
                    SetLinkWeight(link.LinkID, link.Weight);
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return false;
            }
        }

        #endregion

        #region Methods - Dijkstra
        public double[,] MakeMatrix()
        {
            try
            {
                int maxNodeNumber = DatabaseHandler.Instance.DictionaryNodeDataList.Values.Max(item => item.NodeID);
                _DistanceMap = new double[maxNodeNumber + 1, maxNodeNumber + 1];
                _TimeMap = new double[maxNodeNumber + 1, maxNodeNumber + 1];

                for (int from = 0; from < _DistanceMap.GetLength(0); from++)
                {
                    for (int to = 0; to < _DistanceMap.GetLength(1); to++)
                    {
                        _DistanceMap[from, to] = Constants.INFINITE;
                        _TimeMap[from, to] = Constants.INFINITE;
                    }
                }

                foreach (DataItem_Link link in DatabaseHandler.Instance.DictionaryLinkDataList.Values)
                {
                    _DistanceMap[link.FromNodeID, link.ToNodeID] = link.Distance;
                    _TimeMap[link.FromNodeID, link.ToNodeID] = link.Time;
                }

                return _DistanceMap;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<int> GetPath(PathFindMethod findMethod, int source, int destination, bool applyWeight, out double searchTime, out double totalCost)
        {
            try
            {
                List<int> result = new List<int>();

                if (source == destination)
                {
                    result.Add(source);
                    searchTime = 0;
                    totalCost = 0;
                }
                else
                {
                    double cost = 0;
                    totalCost = 0;

                    DateTime startTime = DateTime.Now;

                    double[,] mapInfo = findMethod == PathFindMethod.ByDistance ? (double[,])_DistanceMap.Clone() : (double[,])_TimeMap.Clone();

                    if (applyWeight == true)
                    {
                        // Weight 적용
                        foreach (int linkID in _WeightMap.Keys)
                        {
                            int from = DatabaseHandler.Instance.DictionaryLinkDataList[linkID].FromNodeID;
                            int to = DatabaseHandler.Instance.DictionaryLinkDataList[linkID].ToNodeID;
                            mapInfo[from, to] += _WeightMap[linkID];
                        }
                    }
                    result = DijkstraAlgorithm.MakePathByDijkstra(mapInfo, source, destination, DijkstraAlgorithm.Dijkstra(findMethod, applyWeight, mapInfo, source, source, out cost), out totalCost);

                    searchTime = (DateTime.Now - startTime).TotalMilliseconds;
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                searchTime = 0;
                totalCost = 0;
                return new List<int>();
            }
        }

        public void SetLinkWeight(int linkID, int weightValue)
        {
            try
            {
                if (DatabaseHandler.Instance.DictionaryLinkDataList.ContainsKey(linkID) == true)
                {
                    DatabaseHandler.Instance.DictionaryLinkDataList[linkID].Weight = weightValue;
                }

                if (_WeightMap.ContainsKey(linkID) == false)
                {
                    if (weightValue > 0)
                    {
                        _WeightMap[linkID] = weightValue;
                    }
                }
                else
                {
                    if (weightValue == 0)
                    {
                        _WeightMap.Remove(linkID);
                    }
                    else
                    {
                        _WeightMap[linkID] = weightValue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        /// <summary>
        /// 회피노드 찾기 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public List<int> FindPathAvoidNode(List<int> fullPath, int startNodeNo, Dictionary<int, DataItem_Link> linkDataLists, List<int> avoidPath = null)
        {
            List<int> pathAvoid = new List<int>();
            pathAvoid.Add(startNodeNo);

            int avoidanceAdditionalNodeCount = 0;//ConfigData.Instance.OCSOperation.VehicleAvoidanceAdditionalNodeCount;

            if (avoidPath == null) avoidPath = new List<int>();

            try
            {
                while (true)
                {
                    List<int> curLinks = GetLinkIDsByFrom(startNodeNo, linkDataLists);

                    if (curLinks.Count <= 0) return null;

                    double weight = linkDataLists[curLinks.First()].TotalTimeWeight;
                    int nextNode = -1;

                    foreach (int link in curLinks)
                    {
                        nextNode = linkDataLists[link].ToNodeID;

                        if (avoidPath.Contains(nextNode))
                        {
                            continue;
                        }

                        if (fullPath.Contains(nextNode) == false)
                        {
                            pathAvoid.Add(nextNode);

                            while (avoidanceAdditionalNodeCount > 1)
                            {
                                List<int> nextLinks = GetLinkIDsByFrom(nextNode, linkDataLists);

                                if (nextLinks.Count <= 0)
                                {
                                    return pathAvoid;
                                }

                                double nextWeight = linkDataLists[nextLinks.First()].TotalTimeWeight;
                                nextNode = linkDataLists[nextLinks.First()].ToNodeID;

                                foreach (int nextLink in nextLinks)
                                {
                                    if (linkDataLists[nextLink].TotalTimeWeight < nextWeight)
                                    {
                                        nextWeight = linkDataLists[nextLink].TotalTimeWeight;
                                        nextNode = linkDataLists[nextLink].ToNodeID;
                                    }
                                }

                                pathAvoid.Add(nextNode);
                                avoidanceAdditionalNodeCount--;

                                Thread.Sleep(10);
                            }

                            bool existProhibitLink = false;

                            for (int i = 0; i < pathAvoid.Count - 1; i++)
                            {
                                if (DatabaseHandler.Instance.GetLinkData(pathAvoid[i], pathAvoid[i + 1], linkDataLists).UseFlag == false)
                                {
                                    existProhibitLink = true;
                                }
                            }

                            if (existProhibitLink)
                            {
                                avoidPath.Remove(startNodeNo);
                                return FindPathAvoidNode(fullPath, startNodeNo, linkDataLists, avoidPath);
                            }
                            else
                            {
                                return pathAvoid;
                            }
                        }
                        else
                        {
                            if (weight >= linkDataLists[link].TotalTimeWeight)
                            {
                                weight = linkDataLists[link].TotalTimeWeight;
                                startNodeNo = linkDataLists[link].ToNodeID;
                            }
                        }
                    }

                    pathAvoid.Add(nextNode);
                    startNodeNo = nextNode;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }

        public List<int> GetLinkIDsByFrom(int sourceID, Dictionary<int, DataItem_Link> linkDataLists)
        {
            List<int> linkIDs = new List<int>();

            try
            {
                List<KeyValuePair<int, DataItem_Link>> list = linkDataLists.Where(item => item.Value.FromNodeID == sourceID).ToList();
                if (list.Count > 0)
                {
                    foreach (KeyValuePair<int, DataItem_Link> linkData in list)
                    {
                        linkIDs.Add(linkData.Value.LinkID);
                    }

                    return linkIDs;
                }
                else return null;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        #endregion
    }

}
