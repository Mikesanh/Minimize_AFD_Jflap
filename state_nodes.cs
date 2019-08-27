using System;
using System.Xml;
using System.IO;

public class state
{

    int id;
    string name;
    decimal x_pos;
    decimal y_pos;
    bool initial;
    bool final;
    List<Transition> transitions = new List<Transition>();

    public state(int _id, string _name, decimal x, decimal y, bool _initial, bool _final)
    {
        this.id = _id;
        this.name = _name;
        this.x_pos = x;
        this.y_pos = y;
        this.initial = _initial;
        this.final = _final;

    }




}
public void fill_state()
{
    XmlDocument doc = new XmlDocument();
    doc.Load(textBox1.Text);
    int id;
    string name;
    decimal x_pos;
    decimal y_pos;
    bool initial;
    bool final;

    foreach (XmlNode node in doc.DocumentElement)
    {
        if (node.Name == "automaton")
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "state")
                {
                    //  id = (int)(child.Attributes[0].InnerText);
                    Console.WriteLine("name=" + child.Attributes[1].InnerText);

                    foreach (XmlNode state_child in child.ChildNodes)
                    {

                        if (state_child.Name == "x")
                            Console.WriteLine("X value=" + state_child.InnerText);

                        if (state_child.Name == "y")
                            Console.WriteLine("y value=" + state_child.InnerText);

                        if (state_child.Name == "initial")
                            Console.WriteLine("Initial value= si" + state_child.InnerText);

                        if (state_child.Name == "final")
                            Console.WriteLine("final value= si" + state_child.InnerText);


                    }


                }



            }


        }



    }

}
void add_transition()
{


}

class Transition
{
    int from, to;
    string read;

    Transition(int _from, int _to, string _read)
    {
        this.from = _from;
        this.to = _to;
        this.read = _read;
    }

}