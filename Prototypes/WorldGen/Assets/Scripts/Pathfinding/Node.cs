using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {
        public int g;//Distance from end node
        public int h;
        public Vector2 position;
        public Node parent;

        int heapIndex;

        public Node( Node parent, Vector2 position, int g, int h) {
            this.parent = parent;
            this.position = position;
            this.g = g;
            this.h = h;
        }

        public int HeapIndex {
            get {
                return heapIndex;
            }
            set {
                heapIndex = value;
            }
        }

        public int f {
            get {
                return g + h;
            }
        }

        public int CompareTo(Node nodeToCompare) {
            int compare = f.CompareTo(nodeToCompare.f);
            if (compare == 0) {
                compare = h.CompareTo(nodeToCompare.h);
            }
            return -compare;
        }
    }
