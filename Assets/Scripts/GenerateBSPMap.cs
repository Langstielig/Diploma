using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//x - length
//y - height
//z - width

public class GenerateBSPMap : MonoBehaviour
{
    public GameObject ground1;
    public GameObject wall1;
    public GameObject border;

    private int countOfRooms = 1;
    //private int test = 0;

    void Start()
    {
        // Hall hall = new Hall(0, 0, 8, false, true);
        // hall.HallDrawing(ground1, wall1);

        // Room room = new Room(0, 0, 8, 8, ref countOfRooms);
        // room.RoomDrawing(ground1, wall1);

        // Room room2 = new Room(10, 10, 5, 5, ref countOfRooms);
        // room2.RoomDrawing(ground1, wall1);

        GameObject emptyRooms = new GameObject("Rooms");
        GameObject emptyLeafs = new GameObject("Leafs");

        Leaf leaf = new Leaf(0, 0, 50, 50);
        //leaf.LeafDrawing(border, ref test, ref emptyLeafs);
        leaf.Split();
        leaf.GenerateRooms(ground1, wall1, border, ref countOfRooms, ref emptyRooms, ref emptyLeafs);
    }

    void Update()
    {
        
    }

    public static bool RandomBoolean()
    {
        ///<summary>
        ///returns random boolean value
        ///</summary>
        if(Random.value > 0.5)
        {
            return true;
        }
        else if(Random.value < 0.5)
        {
            return false;
        }
        else
        {
            return RandomBoolean();
        }
    }

    class Leaf
    {
        private const int minLeafSize = 6;

        public int x0;
        public int z0;

        public int width;
        public int length;

        public Leaf leftChild;
        public Leaf rightChild;

        public Room room;
        public Hall hall;

        /// <summary>
        /// constructor without parameters
        /// </summary>
        public Leaf()
        {
            x0 = 0;
            z0 = 0;

            width = 0;
            length = 0;

            leftChild = null;
            rightChild = null;

            room = null;
            hall = null;
        }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="_x">x coordinate</param>
        /// <param name="_z">z coordinate</param>
        /// <param name="l">length (size on x coordinate)</param>
        /// <param name="w">width (size on z coordinate)</param>
        public Leaf(int _x, int _z, int l, int w)
        {
            x0 = _x;
            z0 = _z;

            length = l;
            width = w;

            leftChild = null;
            rightChild = null;
        }

        /// <summary>
        /// function for splitting the space
        /// </summary>
        public void Split()
        {
            Debug.Log("Split leaf: " + x0.ToString() + ", " + z0.ToString() + ", " + width.ToString() + ", " + length.ToString());
            if(leftChild == null && rightChild == null)
            {
                bool horizontalSplitting;

                if(width > length && (double)width / (double)length >= 1.25)
                {
                    horizontalSplitting = true;
                    Debug.Log("width > length");
                }
                else if(length > width && (double)length / (double)width >= 1.25)
                {
                    horizontalSplitting = false;
                    Debug.Log("length > width");
                }
                else
                {
                    horizontalSplitting = RandomBoolean();
                    Debug.Log("random");
                }
                Debug.Log("HorizontalSplitting: " + horizontalSplitting);

                int max;
                if(horizontalSplitting)
                {
                    max = width - minLeafSize;
                }
                else
                {
                    max = length - minLeafSize;
                }

                if(max > minLeafSize)
                {
                    int splitPlace = Random.Range(minLeafSize, max);
                    Debug.Log("SplitPlace: " + splitPlace);
                    if(horizontalSplitting)
                    {
                        leftChild = new Leaf(x0, z0, length, splitPlace);
                        Debug.Log("leftChild: " + x0.ToString() + ", " + z0.ToString() + ", " + length.ToString() + ", " + splitPlace.ToString());
                        rightChild = new Leaf(x0, z0 + splitPlace, length, width - splitPlace);
                        Debug.Log("rightChild: " + x0.ToString() + ", " + (z0 + splitPlace).ToString() + ", " + length.ToString() + ", " + (width - splitPlace).ToString());
                    }
                    else
                    {
                        leftChild = new Leaf(x0, z0, splitPlace, width);
                        //leftChild.LeafDrawing(border, ref leafNumber, ref emptyLeafs);
                        Debug.Log("leftChild: " + x0.ToString() + ", " + z0.ToString() + ", " + splitPlace.ToString() + ", " + width.ToString());
                        rightChild = new Leaf(x0 + splitPlace, z0, length - splitPlace, width);
                        //rightChild.LeafDrawing(border, ref leafNumber,)
                        Debug.Log("rightChild: " + (x0 + splitPlace).ToString() + ", " + z0.ToString() + ", " + (length - splitPlace).ToString() + ", " + width.ToString());
                    }
                }
                else
                {
                    Debug.Log("cant split this leaf because max <= minLeafSize");
                }
                if(leftChild != null)
                {
                    leftChild.Split();
                }
                if(rightChild != null)
                {
                    rightChild.Split();
                }
            }
            else
            {
                Debug.Log("cant split this leaf");
            }
        }

        /// <summary>
        /// function for generating rooms in splitted space
        /// </summary>
        /// <param name="floor">floor prefab</param>
        /// <param name="wall">wall prefab</param>
        /// <param name="border">border prefab</param>
        /// <param name="roomNumber">parameter for counting generated rooms</param>
        /// <param name="emptyRooms">main empty GameObject for hierarchy for storing all generated rooms</param>
        /// <param name="emptyLeafs">main empty GameObject for hierarchy for storing all generated leafs (parts of all space)</param>
        public void GenerateRooms(GameObject floor, GameObject wall, GameObject border, ref int roomNumber,
                                  ref GameObject emptyRooms, ref GameObject emptyLeafs)
        {
            if(leftChild != null || rightChild != null)
            {
                if(leftChild != null)
                {
                    leftChild.GenerateRooms(floor, wall, border, ref roomNumber, ref emptyRooms, ref emptyLeafs);
                }
                if(rightChild != null)
                {
                    rightChild.GenerateRooms(floor, wall, border, ref roomNumber, ref emptyRooms, ref emptyLeafs);
                }
            }
            else
            {
                LeafDrawing(border, ref roomNumber, ref emptyLeafs);

                Debug.Log("create room for leaf: " + x0.ToString() + ", " + z0.ToString() + ", " + width.ToString() + ", " + length.ToString());
                int roomWidth = Random.Range(4, width - 2);
                int roomLength = Random.Range(4, length - 2);
                int roomPosX = Random.Range(1, length - roomLength - 1) + x0;
                int roomPosZ = Random.Range(1, width - roomWidth - 1) + z0;

                room = new Room(roomPosX, roomPosZ, roomLength, roomWidth, roomNumber, ref emptyRooms);
                Debug.Log("roon number: " + roomNumber);
                roomNumber++;
                Debug.Log("generated room: " + roomPosX.ToString() + ", " + roomPosZ.ToString() + ", " + roomLength.ToString() + ", " + roomWidth.ToString());
                room.RoomDrawing(floor, wall);
            }
        }

        /// <summary>
        /// function for drawing borders of created leaf
        /// </summary>
        /// <param name="border">border prefab</param>
        /// <param name="leafNumber">parameter for counting leafs</param>
        /// <param name="emptyLeafs">main empty GameObject for hierarchy for storing all generated leafs (parts of all space</param>
        public void LeafDrawing(GameObject border, ref int leafNumber, ref GameObject emptyLeafs)
        {
            GameObject currentLeaf = new GameObject("Leaf" + leafNumber.ToString());
            currentLeaf.transform.SetParent(emptyLeafs.transform);

            int xPosition = x0;
            int yPosition = -2;
            int zPosition = z0;

            while(xPosition < x0 + length)
            {
                GameObject upperBorder = Instantiate(border);
                upperBorder.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                upperBorder.transform.SetParent(currentLeaf.transform);
                GameObject lowerBorder = Instantiate(border);
                lowerBorder.transform.position = (new Vector3(xPosition, yPosition, zPosition + width - 1));
                lowerBorder.transform.SetParent(currentLeaf.transform);
                xPosition++;
            }

            xPosition = x0;
            zPosition = z0 + 1;

            while(zPosition < z0 + width - 1)
            {
                GameObject leftBorder = Instantiate(border);
                leftBorder.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                leftBorder.transform.SetParent(currentLeaf.transform);
                GameObject rightBorder = Instantiate(border);
                rightBorder.transform.position = (new Vector3(xPosition + length - 1, yPosition, zPosition));
                rightBorder.transform.SetParent(currentLeaf.transform);
                zPosition++;
            }
        }
    }

    class Room 
    {
        public int x0;
        public int z0;
        
        public int width;
        public int length;
        public const int height = 4;

        GameObject emptyRoom;

        public int roomNumber;

        /// <summary>
        /// constructor without parameters
        /// </summary>
        public Room()
        {
            x0 = 0;
            z0 = 0;

            width = 0;
            length = 0; 

            roomNumber = 0;

            emptyRoom = new GameObject("Room" + roomNumber.ToString());
        }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="_x">x coordinate</param>
        /// <param name="_z">z coordinate</param>
        /// <param name="l">length (size on x coordinate)</param>
        /// <param name="w">width (size on z coordinate)</param>
        /// <param name="num">number of room</param>
        /// <param name="emptyRooms">main empty GameObject for hierarchy for storing all generated rooms</param>
        public Room(int _x, int _z, int l, int w, int num, ref GameObject emptyRooms) 
        {
            x0 = _x;
            z0 = _z;

            length = l;
            width = w;

            roomNumber = num;
            num++;

            emptyRoom = new GameObject("Room" + roomNumber.ToString());
            emptyRoom.transform.SetParent(emptyRooms.transform);
        }

        /// <summary>
        /// function for drawing room
        /// </summary>
        /// <param name="floor">floor prefab</param>
        /// <param name="wall">wall prefab</param>
        public void RoomDrawing(GameObject floor, GameObject wall)
        {
            FloorDrawing(floor);
            WallDrawing(wall);
        }

        /// <summary>
        /// function for drawing room floor
        /// </summary>
        /// <param name="floor">floor prefab</param>
        public void FloorDrawing(GameObject floor)
        {
            int xPosition = x0;
            int yPosition = -1;
            int zPosition = z0;

            GameObject emptyFloor = new GameObject("Floor");
            emptyFloor.transform.SetParent(emptyRoom.transform);

            while(zPosition < z0 + width)
            {
                while(xPosition < x0 + length)
                {
                    GameObject lowerFloor = Instantiate(floor);
                    lowerFloor.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                    lowerFloor.transform.SetParent(emptyFloor.transform);
                    GameObject upperFloor = Instantiate(floor);
                    upperFloor.transform.position = (new Vector3(xPosition, yPosition + height + 1, zPosition));
                    upperFloor.transform.SetParent(emptyFloor.transform);

                    xPosition++;
                }

                xPosition = x0;
                zPosition++;
            }
        }

        /// <summary>
        /// function for drawing room walls
        /// </summary>
        /// <param name="wall">wall prefab</param>
        public void WallDrawing(GameObject wall)
        {
            int xPosition = x0;
            int yPosition = 0;
            int zPosition = z0;

            GameObject emptyWall = new GameObject("Walls");
            emptyWall.transform.SetParent(emptyRoom.transform);

            while(xPosition < x0 + length)
            {
                while(yPosition < height)
                {
                    GameObject frontWall = Instantiate(wall);
                    frontWall.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                    frontWall.transform.SetParent(emptyWall.transform);
                    GameObject backWall = Instantiate(wall);
                    backWall.transform.position = (new Vector3(xPosition, yPosition, zPosition + width - 1));
                    backWall.transform.SetParent(emptyWall.transform);

                    yPosition++;
                }
                xPosition++;
                yPosition = 0;
            }

            xPosition = x0;
            yPosition = 0;
            zPosition = z0 + 1;

            while(zPosition < z0 + width)
            {
                while(yPosition < height)
                {
                    GameObject leftWall = Instantiate(wall);
                    leftWall.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                    leftWall.transform.SetParent(emptyWall.transform);
                    GameObject rightWall = Instantiate(wall);
                    rightWall.transform.position = (new Vector3(xPosition + length - 1, yPosition, zPosition));
                    rightWall.transform.SetParent(emptyWall.transform);

                    yPosition++;
                }
                
                zPosition++;
                yPosition = 0;
            }
        }
    }

    class Hall
    {
        public int x0;
        public int z0;

        public int length;
        public const int width = 3;
        public const int height = 4;

        public bool xDirection;
        public bool zDirection;

        /// <summary>
        /// constructor without parameters
        /// </summary>
        public Hall()
        {
            x0 = 0;
            z0 = 0;

            length = 0;

            xDirection = true;
            zDirection = true;
        }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="_x">x coordinate</param>
        /// <param name="_z">z coordinate</param>
        /// <param name="l">size of hall</param>
        /// <param name="hd">bool parameter, showing </param>
        /// <param name="vd"></param>
        public Hall(int _x, int _z, int l, bool xd, bool zd)
        {
            x0 = _x;
            z0 = _z;

            length = l;

            xDirection = xd; //тут хуйня какая-то
            zDirection = zd;
        }

        public void HallDrawing(GameObject floor, GameObject wall)
        {
            GroundDrawing(floor);
            WallDrawing(wall);
        }

        public void GroundDrawing(GameObject floor)
        {
            int counter;
            int xPosition = x0;
            int zPosition = z0;

            int yPosition = -1;

            if(zDirection)
            {
                counter = 1;
            }
            else 
            {
                counter = -1;
            }

            if(xDirection)
            {
                while(xPosition != x0 + length * counter)
                {
                    for(int i = 0; i < width + 2; i++)
                    {
                        GameObject lowerGround = Instantiate(floor);
                        lowerGround.transform.position = (new Vector3(xPosition, yPosition, zPosition + i * counter));
                        GameObject upperGround = Instantiate(floor);
                        upperGround.transform.position = (new Vector3(xPosition, yPosition + height, zPosition + i * counter));
                    }

                    xPosition += counter;
                }
            }
            else
            {
                while(zPosition != z0 + length * counter)
                {
                    for(int i = 0; i < width + 2; i++)
                    {
                        GameObject lowerGround = Instantiate(floor);
                        lowerGround.transform.position = (new Vector3(xPosition + i * counter, yPosition, zPosition));
                        GameObject upperGround = Instantiate(floor);
                        upperGround.transform.position = (new Vector3(xPosition + i * counter, yPosition + height, zPosition));
                    }
                    
                    zPosition += counter;
                }
            }
        }

        public void WallDrawing(GameObject wall)
        {
            int counter;
            int xPosition = x0;
            int zPosition = z0;

            int yPosition = -1;

            if(zDirection)
            {
                counter = 1;
            }
            else 
            {
                counter = -1;
            }

            if(xDirection)
            {
                while(xPosition != x0 + length * counter)
                {
                    for(int i = 1; i < height; i++)
                    {
                        GameObject leftWall = Instantiate(wall);
                        leftWall.transform.position = (new Vector3(xPosition, yPosition + i, zPosition));
                        GameObject rightWall = Instantiate(wall);
                        rightWall.transform.position = (new Vector3(xPosition, yPosition + i, zPosition + counter * (width + 1)));
                    }
                    
                    xPosition += counter;
                }
            }
            else
            {
                while(zPosition != z0 + length * counter) 
                {
                    for(int i = 1; i < height; i++)
                    {
                        GameObject leftHall = Instantiate(wall);
                        leftHall.transform.position = (new Vector3(xPosition, yPosition + i, zPosition));
                        GameObject rightWall = Instantiate(wall);
                        rightWall.transform.position = (new Vector3(xPosition + counter * (width + 1), yPosition + i, zPosition));
                    }

                    zPosition += counter;
                }
            }
        }
    }
}