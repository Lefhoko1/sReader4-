using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
namespace SReader.Game.Classroom
{
    // Local vector illustrations for the bundled practice books. Authored assignment URLs still use HomeUI's loader.
    internal static class ClassroomPracticePictures
    {
        public static bool TryDraw(VisualElement target, string path)
        {
            string name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
            if (path.Contains(":") || path.Contains("/") || path.Contains("\\")) return false;
            if (name != "butterfly" && name != "beetle" && name != "sparrow" && name != "gear" && name != "lever" && name != "wheel") return false;
            var image = new VisualElement { pickingMode = PickingMode.Ignore };
            image.style.flexGrow = 1; target.Add(image);
            image.generateVisualContent += context =>
            {
                var p = context.painter2D; var r = image.contentRect;
                float scale = Mathf.Min(r.width/160,r.height/100);
                Vector2 Point(float x,float y) => new Vector2(r.width/2+(x-80)*scale,r.height/2+(y-50)*scale);
                void Shape(Color color, params Vector2[] points) { p.fillColor=color;p.BeginPath();p.MoveTo(Point(points[0].x,points[0].y));for(int i=1;i<points.Length;i++)p.LineTo(Point(points[i].x,points[i].y));p.ClosePath();p.Fill(); }
                void Oval(Color color,float x,float y,float rx,float ry) { var points=new Vector2[32];for(int i=0;i<32;i++){float a=i*Mathf.PI/16;points[i]=new Vector2(x+Mathf.Cos(a)*rx,y+Mathf.Sin(a)*ry);}Shape(color,points); }
                void Line(Color color,float x,float y,float x2,float y2,float width=2) {p.strokeColor=color;p.lineWidth=width*scale;p.BeginPath();p.MoveTo(Point(x,y));p.LineTo(Point(x2,y2));p.Stroke();}
                Color ink=new Color(.17f,.22f,.23f),gold=new Color(.8f,.49f,.16f),green=new Color(.26f,.42f,.26f),paper=new Color(.96f,.92f,.81f);
                if(name=="butterfly")
                {Oval(gold,57,35,23,24);Oval(gold,103,35,23,24);Oval(new Color(.65f,.3f,.18f),59,65,20,21);Oval(new Color(.65f,.3f,.18f),101,65,20,21);Oval(ink,80,49,5,31);Line(ink,79,23,70,12);Line(ink,81,23,90,12);Oval(paper,53,32,8,10);Oval(paper,107,32,8,10);}
                else if(name=="beetle")
                {for(int y=35;y<=65;y+=15){Line(ink,65,y,42,y-9);Line(ink,95,y,118,y-9);}Oval(green,80,54,25,30);Oval(ink,80,25,12,12);Line(ink,80,26,80,83);Line(ink,75,18,65,9);Line(ink,85,18,95,9);}
                else if(name=="sparrow")
                {Shape(ink,new Vector2(60,65),new Vector2(30,45),new Vector2(39,72),new Vector2(68,73));Oval(new Color(.53f,.35f,.23f),83,57,33,23);Oval(gold,108,34,17,18);Oval(ink,113,31,3,3);Shape(gold,new Vector2(121,33),new Vector2(137,38),new Vector2(121,42));Oval(ink,78,55,22,11);Line(ink,77,75,73,89);Line(ink,95,76,99,89);Line(ink,67,90,81,90);Line(ink,92,90,106,90);}
                else if(name=="lever")
                {Shape(green,new Vector2(65,85),new Vector2(80,53),new Vector2(95,85));Line(ink,28,57,135,33,8);Shape(gold,new Vector2(24,42),new Vector2(43,38),new Vector2(46,54),new Vector2(27,58));}
                else
                {if(name=="gear")for(int i=0;i<12;i++){float a=i*Mathf.PI/6;Oval(ink,80+Mathf.Cos(a)*34,50+Mathf.Sin(a)*34,7,7);}Oval(ink,80,50,33,33);Oval(gold,80,50,24,24);for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Line(ink,80,50,80+Mathf.Cos(a)*25,50+Mathf.Sin(a)*25,4);}Oval(paper,80,50,7,7);}
            };
            return true;
        }
    }
}
