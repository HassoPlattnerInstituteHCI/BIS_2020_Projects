using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
using SpeechIO;

namespace PantoDrawing
{
    public class LineDraw : MonoBehaviour
    {

        public GameObject linePrefab;
        public GameObject currentLine;
        public bool canDraw = false;

        UpperHandle upperHandle;
        LowerHandle lowerHandle;

        float upperRotation;
        float angle;
        public int lineCount = 0;
        //currently adding points to a line
        bool drawing = false;
        public bool mouse;

        float handleVelocity = .1f;

        public LineRenderer lineRenderer;

        public List<Vector3> fingerPositions;

        public Dictionary<string, LineRenderer> lines = new Dictionary<string, LineRenderer>();

        public Audio audio;

        // Start is called before the first frame update
        void Start()
        {
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            upperRotation = upperHandle.GetRotation();
            audio = GameObject.Find("Panto").GetComponent<Audio>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!canDraw)
                return;
            angle = Mathf.Abs(upperHandle.GetRotation() - upperRotation);
            angle = Mathf.Abs((angle + 180) % 360 - 180);
            if((angle > 80f  && !mouse && !drawing) || (Input.GetMouseButtonDown(0) && mouse)) 
            {
                upperRotation = upperHandle.GetRotation();
                angle = upperHandle.GetRotation() - upperRotation;
                angle = Mathf.Abs((angle + 180) % 360 - 180);
                CreateLine();
                drawing = true;
                audio.drawingSound();
            }
            if((angle < 30f && !mouse && drawing) || (Input.GetMouseButton(0) && mouse)){
                Vector3 tempFingerPos = upperHandle.HandlePosition(transform.position);
                tempFingerPos.y = .1f;
                Vector3 latestPos = fingerPositions[fingerPositions.Count - 1];
                latestPos.y = .1f;
                if(Vector3.Distance(tempFingerPos, latestPos) > .1f)
                {
                    UpdateLine(lineRenderer, tempFingerPos);
                }
            }else{
                if(drawing)
                {
                    string lineName = "line"+lineCount;
                    Vector3[] linePos = new Vector3[lineRenderer.positionCount];
                    lineRenderer.GetPositions(linePos);
                    lineRenderer.SetPositions(Curver.MakeSmoothCurve(linePos, .5f));
                    lines.Add(lineName, lineRenderer);
                    lineRenderer.name = lineName;
                    GameObject.Find("Panto").GetComponent<GameManager>().AddVoiceCommand( (lineCount+1)+"" , () =>
                    {
                        TraceLine(lines[lineName]);
                    });
                    lineCount++;
                    drawing = false;
                    audio.stopSound();
                }
            }
        }

        LineRenderer CreateLine()
        {   
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            fingerPositions.Clear();
            Vector3 newPos = upperHandle.HandlePosition(transform.position);
            newPos.y = .1f;
            fingerPositions.Add(newPos);
            fingerPositions.Add(newPos);
            lineRenderer.SetPosition(0, fingerPositions[0]);
            lineRenderer.SetPosition(1, fingerPositions[1]);
            return lineRenderer;
        }

        

        public void CreateRectangle()
        {
            LineRenderer line = lines["line"+(lineCount-1)];
            RectangleDraw.CreateRectanglePoints(line);
        }

        public void CreateTriangle()
        {
            LineRenderer line = lines["line"+(lineCount-1)];
            TriangleDraw.CreateTrianglePoints(line);
        }

        public void CreateCircle(){
            LineRenderer line = lines["line"+(lineCount-1)];
            CircleDraw.CreateCircle(line);
        }

        void UpdateLine(LineRenderer line, Vector3 newFingerPos)
        {
            fingerPositions.Add(newFingerPos);
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, newFingerPos);
        }

        public async Task TraceLine(LineRenderer line)
        {
            Vector3[] linePos = new Vector3[line.positionCount];
            line.GetPositions(linePos);
            for (int i = 0; i < line.positionCount; i += 3)
            {
                await lowerHandle.MoveToPosition(linePos[i], handleVelocity);
            }
            Debug.Log(linePos[0]);
        }

        public async void FindStartingPoint(LineRenderer line)
        {
            Vector3[] linePos = new Vector3[line.positionCount];
            line.GetPositions(linePos);
            await lowerHandle.MoveToPosition(linePos[0], handleVelocity);
        }

        public async Task ShowLines()
        {
            foreach (KeyValuePair<string, LineRenderer> line in lines)
            {
                await TraceLine(line.Value);
            }
        }

        public void CombineLines(LineRenderer line1, LineRenderer line2, bool inverted = false)
        {
            Vector3[] line2Pos = new Vector3[line2.positionCount];
            line2.GetPositions(line2Pos);

            if (!inverted){
               for(int i = 0; i < line2.positionCount; i++){
                line1.positionCount++;
                line1.SetPosition(line1.positionCount - 1, line2Pos[i]);
                } 
            } else {
                for(int i = line2.positionCount-1; i >= 0; i--){
                line1.positionCount++;
                line1.SetPosition(line1.positionCount - 1, line2Pos[i]);
                } 
            }
        }

        public void ResetDrawingArea()
        {
            foreach (KeyValuePair<string, LineRenderer> line in lines)
            {
                Destroy(line.Value);
            }
            lines = new Dictionary<string, LineRenderer>();
            lineCount = 0;
        }
    }
}