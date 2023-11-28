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
        GameObject emptyRooms = new GameObject("Rooms");
        GameObject emptyHalls = new GameObject("Halls");
        GameObject emptyLeafs = new GameObject("Leafs");

        // Hall hall = new Hall(0, 0, 8, false, true);
        // hall.HallDrawing(ground1, wall1);

        // Room room = new Room(0, 0, 8, 8, ref countOfRooms);
        // room.RoomDrawing(ground1, wall1);

        // Room room2 = new Room(10, 10, 5, 5, ref countOfRooms);
        // room2.RoomDrawing(ground1, wall1);

        //Hall hall1 = new Hall(11, 2, 8, true, 1, 2, ref emptyHalls);
        //hall.HallDrawing(ground1, wall1);

        //Hall hall2 = new Hall(3, 8, 8, false, 1, 2, ref emptyHalls);

        //Room room = new Room(0, 0, 11, 8, 1, ref emptyRooms, hall1, hall2);
        //room.RoomDrawing(ground1, wall1);

        Leaf leaf = new Leaf(0, 0, 10, 21);
        leaf.Split();
        leaf.GenerateRooms(border, ref countOfRooms, ref emptyRooms, ref emptyHalls, ref emptyLeafs);
        leaf.GenerateHalls(ref emptyHalls);
        leaf.DrawMap(ground1, wall1);

        //Leaf leaf = new Leaf(0, 0, 50, 50);
        ////leaf.LeafDrawing(border, ref test, ref emptyLeafs);
        //leaf.Split();
        //leaf.GenerateRooms(ground1, wall1, border, ref countOfRooms, ref emptyRooms, ref emptyHalls, ref emptyLeafs);

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
        private const int minLeafSize = 10;

        public int x0;
        public int z0;

        public int width;
        public int length;

        public Leaf leftChild;
        public Leaf rightChild;

        public bool isHorizontalSplitting;
        public bool isSplitted;

        public Room room;

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

            isHorizontalSplitting = true;
            isSplitted = false;

            room = null;
        }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="_x">x coordinate of lower left corner</param>
        /// <param name="_z">z coordinate of lower left corner</param>
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

            isHorizontalSplitting = true;
            isSplitted = false;

            room = null;
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
                        isHorizontalSplitting = true;
                        isSplitted = true;
                        leftChild = new Leaf(x0, z0, length, splitPlace);
                        Debug.Log("leftChild: " + x0.ToString() + ", " + z0.ToString() + ", " + length.ToString() + ", " + splitPlace.ToString());
                        rightChild = new Leaf(x0, z0 + splitPlace, length, width - splitPlace);
                        Debug.Log("rightChild: " + x0.ToString() + ", " + (z0 + splitPlace).ToString() + ", " + length.ToString() + ", " + (width - splitPlace).ToString());
                    }
                    else
                    {
                        isHorizontalSplitting = false;
                        isSplitted = true;
                        leftChild = new Leaf(x0, z0, splitPlace, width);
                        Debug.Log("leftChild: " + x0.ToString() + ", " + z0.ToString() + ", " + splitPlace.ToString() + ", " + width.ToString());
                        rightChild = new Leaf(x0 + splitPlace, z0, length - splitPlace, width);
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
        /// <param name="emptyHalls">main empty GameObject for hierarchy for storing all generated halls</param>
        /// <param name="emptyLeafs">main empty GameObject for hierarchy for storing all generated leafs (parts of all space)</param>
        public void GenerateRooms(GameObject border, ref int roomNumber,
                                  ref GameObject emptyRooms, ref GameObject emptyHalls, ref GameObject emptyLeafs) //��� ����� ����� ������ ������ ���������, ������ ��� ����� �� ����� ����� ��������
        {
            if(leftChild != null || rightChild != null)
            {
                if(leftChild != null)
                {
                    leftChild.GenerateRooms(border, ref roomNumber, ref emptyRooms, ref emptyHalls, ref emptyLeafs);
                }
                if(rightChild != null)
                {
                    rightChild.GenerateRooms(border, ref roomNumber, ref emptyRooms, ref emptyHalls, ref emptyLeafs);
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
                Debug.Log("room number: " + roomNumber);
                roomNumber++;
                Debug.Log("generated room: " + roomPosX.ToString() + ", " + roomPosZ.ToString() + ", " + roomLength.ToString() + ", " + roomWidth.ToString());
                //room.RoomDrawing(floor, wall);
            }
        }

        public void GenerateHalls(ref GameObject emptyHalls)
        {
            if(leftChild != null && leftChild.room != null && rightChild != null && rightChild.room != null)
            {
                int min;
                int max;
                int xPosition;
                int zPosition;
                int lengthOfHall;
                if (isHorizontalSplitting)
                {
                    if (leftChild.room.x0 > rightChild.room.x0)
                        min = leftChild.room.x0;
                    else
                        min = rightChild.room.x0;

                    if ((leftChild.room.x0 + leftChild.room.length) < (rightChild.room.x0 + rightChild.room.length))
                        max = leftChild.x0 + leftChild.room.length - 1;
                    else
                        max = rightChild.room.x0 + rightChild.room.length - 1;

                    xPosition = Random.Range(min, max); //��� ������ ����� ����� max + 1
                    zPosition = leftChild.room.z0 + leftChild.room.width;
                    lengthOfHall = rightChild.room.z0 - zPosition;
                }
                else
                {
                    if (leftChild.room.z0 > rightChild.room.z0)
                        min = leftChild.room.z0;
                    else
                        min = rightChild.room.z0;

                    if ((leftChild.room.z0 + leftChild.room.width) < (rightChild.room.z0 + rightChild.room.width))
                        max = leftChild.room.z0 + leftChild.room.width - 1;
                    else
                        max = rightChild.room.z0 + rightChild.room.width - 1;

                    xPosition = leftChild.room.x0 + leftChild.room.length;
                    zPosition = Random.Range(min, max); // ��� ���� max + 1
                    lengthOfHall = rightChild.room.x0 - xPosition;
                }
                Hall newHall = new Hall(xPosition, zPosition, lengthOfHall, false, leftChild.room.roomNumber, rightChild.room.roomNumber, ref emptyHalls);
                leftChild.room.AddHall(newHall);
            }
            else
            {
                if (leftChild != null)
                    leftChild.GenerateHalls(ref emptyHalls);
                if (rightChild != null)
                    rightChild.GenerateHalls(ref emptyHalls);
            }
        }

        public void DrawMap(GameObject floor, GameObject wall)
        {
            if (room != null)
            {
                //LeafDrawing(border, ref roomNumber, ref emptyLeafs);
                room.RoomDrawing(floor, wall);
            }
            else
            {
                if (leftChild != null)
                    leftChild.DrawMap(floor, wall);
                if (rightChild != null)
                    rightChild.DrawMap(floor, wall);
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

        Hall horizontalHall;
        Hall verticalHall;

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

            horizontalHall = null;
            verticalHall = null;
        }

        /// <summary>
        /// constructor with parameters without halls
        /// </summary>
        /// <param name="_x">x coordinate of lower left corner</param>
        /// <param name="_z">z coordinate of lower left corner</param>
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
        /// constructor with parameters with one hall
        /// </summary>
        /// <param name="_x">x coordinate of lower left corner</param>
        /// <param name="_z">z coordinate of lower left corner</param>
        /// <param name="l">length (size on x coordinate)</param>
        /// <param name="w">width (size on z coordinate)</param>
        /// <param name="num">number of room</param>
        /// <param name="emptyRooms">main empty GameObject for hierarchy for storing all generated rooms</param>
        /// <param name="h">horizontal or vertical hall</param>
        public Room(int _x, int _z, int l, int w, int num, ref GameObject emptyRooms, Hall h)
        {
            x0 = _x;
            z0 = _z;

            length = l;
            width = w;

            roomNumber = num;
            num++;

            emptyRoom = new GameObject("Room" + roomNumber.ToString());
            emptyRoom.transform.SetParent(emptyRooms.transform);

            if (h.isHorizontal)
            {
                horizontalHall = h;
                verticalHall = null;
            }
            else
            {
                verticalHall = h;
                horizontalHall = null;
            }
        }

        /// <summary>
        /// constructor with parameters with both halls
        /// </summary>
        /// <param name="_x">x coordinate of lower left corner</param>
        /// <param name="_z">z coordinate of lower left corner</param>
        /// <param name="l">length (size on x coordinate)</param>
        /// <param name="w">width (size on z coordinate)</param>
        /// <param name="num">number of room</param>
        /// <param name="emptyRooms">main empty GameObject for hierarchy for storing all generated rooms</param>
        /// <param name="horizontal">horizontal hall</param>
        /// <param name="vertical">vertical hall</param>
        public Room(int _x, int _z, int l, int w, int num, ref GameObject emptyRooms, Hall horizontal, Hall vertical)
        {
            x0 = _x;
            z0 = _z;

            length = l;
            width = w;

            roomNumber = num;
            num++;

            emptyRoom = new GameObject("Room" + roomNumber.ToString());
            emptyRoom.transform.SetParent(emptyRooms.transform);

            horizontalHall = horizontal;
            verticalHall = vertical;
        }

        public void AddHall(Hall hall)
        {
            if (hall.isHorizontal)
                horizontalHall = hall;
            else
                verticalHall = hall;
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

            if(horizontalHall != null)
                horizontalHall.HallDrawing(floor, wall);

            if(verticalHall != null)
                verticalHall.HallDrawing(floor, wall);
        }

        /// <summary>
        /// function for drawing room floor
        /// </summary>
        /// <param name="floor">floor prefab</param>
        private void FloorDrawing(GameObject floor)
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
                    //��������� ��������
                    //GameObject upperFloor = Instantiate(floor);
                    //upperFloor.transform.position = (new Vector3(xPosition, yPosition + height + 1, zPosition));
                    //upperFloor.transform.SetParent(emptyFloor.transform);

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
        private void WallDrawing(GameObject wall)
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
                    GameObject lowerWall = Instantiate(wall);
                    lowerWall.transform.position = (new Vector3(xPosition, yPosition, zPosition));
                    lowerWall.transform.SetParent(emptyWall.transform);
                    if (verticalHall == null || verticalHall != null && !(xPosition > verticalHall.x0 && xPosition < (verticalHall.x0 + verticalHall.width - 1)))
                    {
                        GameObject upperWall = Instantiate(wall);
                        upperWall.transform.position = (new Vector3(xPosition, yPosition, zPosition + width - 1));
                        upperWall.transform.SetParent(emptyWall.transform);
                    }
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
                    if (horizontalHall == null || horizontalHall != null && !(zPosition > horizontalHall.z0 && zPosition < (horizontalHall.z0 + horizontalHall.width - 1)))
                    {
                        GameObject rightWall = Instantiate(wall);
                        rightWall.transform.position = (new Vector3(xPosition + length - 1, yPosition, zPosition));
                        rightWall.transform.SetParent(emptyWall.transform);
                    }
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
        public int width = 4;
        public const int height = 4;

        public bool isHorizontal;

        GameObject emptyHall;

        public int leftRoomNumber;
        public int rightRoomNumber;

        /// <summary>
        /// constructor without parameters
        /// </summary>
        public Hall()
        {
            x0 = 0;
            z0 = 0;

            length = 0;

            isHorizontal = true;

            leftRoomNumber = 0;
            rightRoomNumber = 0;

            emptyHall = new GameObject("Hall" + leftRoomNumber.ToString() + rightRoomNumber.ToString());
        }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="_x">x coordinate of lower left corner</param>
        /// <param name="_z">z coordinate of lower left corner</param>
        /// <param name="l">size of hall</param>
        /// <param name="iH">parameter, showing is hall horizontal (true) or vertical (false)</param>
        /// <param name="leftNum">number of left room, connected with this hall</param>
        /// <param name="rightNum">number or right room, connected with this hall</param>
        /// <param name="emptyHalls">main empty GameObject for hierarchy for storing all generated rooms</param>
        public Hall(int _x, int _z, int l, bool iH, int leftNum, int rightNum, ref GameObject emptyHalls)
        {
            x0 = _x;
            z0 = _z;

            length = l;

            isHorizontal = iH;

            leftRoomNumber = leftNum;
            rightRoomNumber = rightNum;

            emptyHall = new GameObject("Hall" + leftRoomNumber.ToString() + rightRoomNumber.ToString());
            emptyHall.transform.SetParent(emptyHall.transform);
        }

        /// <summary>
        /// function for drawing hall
        /// </summary>
        /// <param name="floor">floor prefab</param>
        /// <param name="wall">wall prefab</param>
        public void HallDrawing(GameObject floor, GameObject wall)
        {
            GroundDrawing(floor);
            WallDrawing(wall);
        }

        /// <summary>
        /// function for drawing hall ground and ceiling
        /// </summary>
        /// <param name="floor">floor prefab</param>
        private void GroundDrawing(GameObject floor)
        {
            int xPosition = x0;
            int yPosition = -1;
            int zPosition = z0;

            if(isHorizontal)
            {
                while(xPosition < x0 + length)
                {
                    for(int i = z0; i < z0 + width; i++)
                    {
                        GameObject currentGround = Instantiate(floor);
                        currentGround.transform.position = new Vector3(xPosition, yPosition, i);
                        currentGround.transform.SetParent(emptyHall.transform);
                        //GameObject currentCeiling = Instantiate(floor);
                        //currentCeiling.transform.position = new Vector3(xPosition, yPosition + height + 1, i);
                        //currentCeiling.transform.SetParent(emptyHall.transform);
                    }
                    xPosition++;
                }
            }
            else
            {
                while(zPosition < z0 + length)
                {
                    for(int i = x0; i < x0 + width; i++)
                    {
                        GameObject currentGround = Instantiate(floor);
                        currentGround.transform.position = new Vector3(i, yPosition, zPosition);
                        currentGround.transform.SetParent(emptyHall.transform);
                        //GameObject currentCeiling = Instantiate(floor);
                        //currentCeiling.transform.position = new Vector3(i, yPosition + height + 1, zPosition);
                        //currentCeiling.transform.SetParent(emptyHall.transform);
                    }
                    zPosition++;
                }
            }
        }

        /// <summary>
        /// function for drawing hall walls
        /// </summary>
        /// <param name="wall">wall prefab</param>
        private void WallDrawing(GameObject wall)
        {
            int xPosition = x0;
            int yPosition = 0;
            int zPosition = z0;

            if(isHorizontal)
            {
                while(xPosition < x0 + length)
                {
                    for(int i = yPosition; i < yPosition + height; i++)
                    {
                        GameObject lowerWall = Instantiate(wall);
                        lowerWall.transform.position = (new Vector3(xPosition, i, zPosition));
                        lowerWall.transform.SetParent(emptyHall.transform);
                        GameObject upperWall = Instantiate(wall);
                        upperWall.transform.position = (new Vector3(xPosition, i, zPosition + width - 1));
                        upperWall.transform.SetParent(emptyHall.transform);
                    }
                    xPosition++;
                }
            }
            else
            {
                while(zPosition < z0 + length)
                {
                    for(int i = yPosition; i < yPosition + height; i++)
                    {
                        GameObject leftWall = Instantiate(wall);
                        leftWall.transform.position = (new Vector3(xPosition, i, zPosition));
                        leftWall.transform.SetParent(emptyHall.transform);
                        GameObject rightWall = Instantiate(wall);
                        rightWall.transform.position = (new Vector3(xPosition + width - 1, i, zPosition));
                        rightWall.transform.SetParent(emptyHall.transform);
                    }
                    zPosition++;
                }
            }
        }
    }
}