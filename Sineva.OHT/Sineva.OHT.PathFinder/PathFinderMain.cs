using Sineva.OHT.Common;
using Sineva.VHL.Common;
using Sineva.VHL.Data;
//using Sineva.OCS.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sineva.OHT.PathFinder
{
    public class PathFinderMain : Singleton<PathFinderMain>
    {
        #region Fields
        private VehicleData _Data = VehicleData.Instance;
        private double[,] _DistanceMap;
        private double[,] _TimeMap;
        private Dictionary<int, int> _WeightMap = new Dictionary<int, int>();
        private const int INFINITE = 100000000;
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

                foreach (DataItem_Link link in _Data.LinkDataList.Values)
                {
                    SetLinkWeight(link.LinkID, link.Weight);
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return false;
            }
        }

        #endregion

        #region Methods - Dijkstra
        public double[,] MakeMatrix()
        {
            try
            {
                int maxNodeNumber = _Data.NodeDataList.Values.Max(item => item.NodeID);
                _DistanceMap = new double[maxNodeNumber + 1, maxNodeNumber + 1];
                _TimeMap = new double[maxNodeNumber + 1, maxNodeNumber + 1];

                for (int from = 0; from < _DistanceMap.GetLength(0); from++)
                {
                    for (int to = 0; to < _DistanceMap.GetLength(1); to++)
                    {
                        _DistanceMap[from, to] = INFINITE;
                        _TimeMap[from, to] = INFINITE;
                    }
                }

                foreach (DataItem_Link link in _Data.LinkDataList.Values)
                {
                    _DistanceMap[link.FromNodeID, link.ToNodeID] = link.Distance;
                    _TimeMap[link.FromNodeID, link.ToNodeID] = link.Time;
                }

                return _DistanceMap;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
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
                            int from = _Data.LinkDataList[linkID].FromNodeID;
                            int to = _Data.LinkDataList[linkID].ToNodeID;
                            mapInfo[from, to] += _WeightMap[linkID];
                        }
                    }

                    result = MakePathByDijkstra(mapInfo, source, destination, Dijkstra(findMethod, applyWeight, mapInfo, source, source, out cost), out totalCost);

                    searchTime = (DateTime.Now - startTime).TotalMilliseconds;
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                searchTime = 0;
                totalCost = 0;
                return new List<int>();
            }
        }

        public List<int> GetPathOnlyBranchNextNode(List<int> fullPath, int source, int destination)
        {
            try
            {
                List<int> path = new List<int>();

                bool preNodeIsBranchNode = false;

                for (int i = 0; i < fullPath.Count; i++)
                {
                    int currentNode = fullPath[i];

                    if (currentNode == source) path.Add(currentNode);

                    if (_Data.GetNodeDataOrNull(currentNode) != null && _Data.GetNodeDataOrNull(currentNode).Type == NodeType.Branch)
                    {
                        preNodeIsBranchNode = true;
                    }
                    else
                    {
                        if (preNodeIsBranchNode)
                        {
                            preNodeIsBranchNode = false;
                            path.Add(currentNode);
                        }
                    }

                    if (i == fullPath.Count - 1 && path.Contains(currentNode) == false)
                    {
                        path.Add(currentNode);
                    }
                }

                return path;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }

        public List<int> GetMoveWeightNodes(List<int> path)
        {
            List<int> moveWeightNodes = new List<int>();

            try
            {
                foreach (int node in path)
                {





                }
            }
            catch (Exception ex)
            {

            }

            return moveWeightNodes;
        }

        public void SetLinkWeight(int source, int destination, int weightValue)
        {
            try
            {
                int linkID = GetLinkID(source, destination);

                if (linkID == -1) return;

                if (_Data.LinkDataList.ContainsKey(linkID) == true)
                {
                    _Data.LinkDataList[linkID].Weight = weightValue;
                    //Query_Link.Instance.Update(_Data.RTData.LinkDatas[linkID]);
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
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        public void SetLinkWeight(int linkID, int weightValue)
        {
            try
            {
                if (_Data.LinkDataList.ContainsKey(linkID) == true)
                {
                    _Data.LinkDataList[linkID].Weight = weightValue;
                    //Query_Link.Instance.Update(_Data.RTData.LinkDatas[linkID]);
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
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }

        private int[] Dijkstra(PathFindMethod findMethod, bool applyWeight, double[,] matrixArray, int sourceIndex, int destinationIndex, out double totalCost)
        {
            try
            {
                int length = matrixArray.GetLength(0);
                double[] costData = new double[length];
                bool[] blockData = new bool[length];
                int[] parentInfo = new int[length];
                for (int i = 0; i < length; i++)
                {
                    costData[i] = INFINITE;
                    blockData[i] = false;
                    parentInfo[i] = -1;
                }

                costData[sourceIndex] = 0;

                int current = sourceIndex;

                //foreach (int from in matrixArray.Keys)
                for (int from = 0; from < length; from++)
                {
                    double min = INFINITE;
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
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                totalCost = INFINITE;
                return new int[0];
            }
        }

        protected List<int> MakePathByDijkstra(double[,] mapInfo, int sourceIndex, int destinationIndex, int[] parentArray, out double totalCost)
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

                    int linkID = GetLinkID(current, old);

                    try
                    {
                        totalCost += mapInfo[current, old];
                    }
                    catch (Exception ex)
                    {
                        System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                        _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                        totalCost = 0;
                        return new List<int>();
                    }

                    if (current < 0)
                    {
                        shortestPath.Clear();
                        return shortestPath;
                    }
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
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                totalCost = 0;
                return new List<int>();
            }
        }

        private int GetLinkID(int sourceID, int destID)
        {
            try
            {
                List<KeyValuePair<int, DataItem_Link>> list = _Data.LinkDataList.Where(item => item.Value.FromNodeID == sourceID && item.Value.ToNodeID == destID).ToList();
                if (list.Count > 0) return list.First().Value.LinkID;

                return -1;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }

        /// <summary>
        /// 회피노드 찾기 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public List<int> FindPathAvoidNode(List<int> fullPath, int startNodeNo, List<int> avoidPath = null)
        {
            List<int> pathAvoid = new List<int>();
            pathAvoid.Add(startNodeNo);

            int avoidanceAdditionalNodeCount = 0;//ConfigData.Instance.OCSOperation.VehicleAvoidanceAdditionalNodeCount;

            if (avoidPath == null) avoidPath = new List<int>();

            try
            {
                while (true)
                {
                    List<int> curLinks = _Data.GetLinkIDsByFrom(startNodeNo);

                    if (curLinks.Count <= 0) return null;

                    double weight = _Data.LinkDataList[curLinks.First()].TotalTimeWeight;
                    int nextNode = -1;

                    foreach (int link in curLinks)
                    {
                        nextNode = _Data.LinkDataList[link].ToNodeID;

                        if (avoidPath.Contains(nextNode))
                        {
                            continue;
                        }

                        if (fullPath.Contains(nextNode) == false)
                        {
                            pathAvoid.Add(nextNode);

                            while (avoidanceAdditionalNodeCount > 1)
                            {
                                List<int> nextLinks = _Data.GetLinkIDsByFrom(nextNode);

                                if (nextLinks.Count <= 0)
                                {
                                    return pathAvoid;
                                }

                                double nextWeight = _Data.LinkDataList[nextLinks.First()].TotalTimeWeight;
                                nextNode = _Data.LinkDataList[nextLinks.First()].ToNodeID;

                                foreach (int nextLink in nextLinks)
                                {
                                    if (_Data.LinkDataList[nextLink].TotalTimeWeight < nextWeight)
                                    {
                                        nextWeight = _Data.LinkDataList[nextLink].TotalTimeWeight;
                                        nextNode = _Data.LinkDataList[nextLink].ToNodeID;
                                    }
                                }

                                pathAvoid.Add(nextNode);
                                avoidanceAdditionalNodeCount--;

                                Thread.Sleep(10);
                            }

                            bool existProhibitLink = false;

                            for (int i = 0; i < pathAvoid.Count - 1; i++)
                            {
                                if (_Data.GetLinkData(pathAvoid[i], pathAvoid[i + 1]).UseFlag == false)
                                {
                                    existProhibitLink = true;
                                }
                            }

                            if (existProhibitLink)
                            {
                                avoidPath.Remove(startNodeNo);
                                return FindPathAvoidNode(fullPath, startNodeNo, avoidPath);
                            }
                            else
                            {
                                return pathAvoid;
                            }
                        }
                        else
                        {
                            if (weight >= _Data.LinkDataList[link].TotalTimeWeight)
                            {
                                weight = _Data.LinkDataList[link].TotalTimeWeight;
                                startNodeNo = _Data.LinkDataList[link].ToNodeID;
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
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }

        public List<int> FindPathParkingNode(int startNodeNo, bool findOtherCPSZone = false, bool findSameZone = false)
        {
            List<int> pathParkingNode = new List<int>();

            try
            {
                // Vehicle 없는 Parking Node만 가져옴  //  
                List<KeyValuePair<int, DataItem_Node>> parkingNodeList = _Data.NodeDataList.Where(item => item.Value.Type == NodeType.Park && item.Value.NodeID != startNodeNo).ToList();

                // Parking Node를 가져옴
                //List<KeyValuePair<int, DataItem_Node>> parkingNodeList = _Data.RTData.NodeDatas.Where(item => item.Value.Type == NodeType.Park).ToList();

                Dictionary<double, List<int>> pathTimeDic = new Dictionary<double, List<int>>();

                foreach (KeyValuePair<int, DataItem_Node> parkingNode in parkingNodeList)
                {
                    List<int> path = GetPath(PathFindMethod.ByTime, startNodeNo, parkingNode.Value.NodeID, true, out double searchTime, out double totalCost);

                    // 다른 Vehicle이 없는 Parking Node 찾기를 해야하는가? 없는 경우 다시 찾기 해야하나?
                    // 가까운 Parking Node의 Vehicle을 다른 Parking Node로 보내야하나?

                    // 다른 CPS Zone의 Parking Node
                    if (findOtherCPSZone)
                    {
                        DataItem_Node node = _Data.GetNodeDataOrNull(startNodeNo);
                        if (parkingNode.Value.CPSZoneID != node.CPSZoneID)
                        {
                            pathTimeDic[totalCost] = path;
                        }
                    }
                    else
                    {
                        pathTimeDic[totalCost] = path;
                    }
                }

                if (pathTimeDic.Count > 0)
                {
                    pathParkingNode = pathTimeDic.OrderBy(item => item.Key).First().Value;
                }

                return pathParkingNode;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                _Data.SetExceptionLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                pathParkingNode.Clear();
                return pathParkingNode;
            }
        }
        #endregion
    }
}
