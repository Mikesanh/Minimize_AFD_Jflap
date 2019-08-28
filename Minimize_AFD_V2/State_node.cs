using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows.Forms;


namespace Minimize_AFD_V2
{

    public class State_node
    {
        byte id;
        string name;
        float x_pos;
        float y_pos;
        bool initial = false;
        bool final = false;
        public List<Transition> transitions = new List<Transition>();
        //List<Transition> transition_list = new List<Transition>();

        public State_node(byte _id, string _name, float x, float y, bool _initial, bool _final)
        {
            id = _id;
            name = _name;
            x_pos = x;
            y_pos = y;
            initial = _initial;
            final = _final;

        }
        public List<Transition> get_trans()
        {
            return transitions;
        }
        public State_node()
        {

        }
        public byte getId()
        {
            return id;
        }
        public string getName()
        {
            return name;
        }
        public float getX()
        {
            return x_pos;
        }
        public float getY()
        {
            return y_pos;
        }
        public bool getInit()
        {
            return initial;
        }
        public bool getFinal()
        {
            return final;
        }

        public void setId(byte _id)
        {
            id = _id;
        }
        public void setName(string _name)
        {
            name = _name;
        }
        public void setX(float _x)
        {
            x_pos = _x;
        }
        public void setY(float _y)
        {
            y_pos = _y;
        }
        public void setInit(bool _initial)
        {
            initial = _initial;
        }
        public void setFinal(bool _final)
        {
            final = _final;
        }

        //public List<Transition> get_transition_list()
        //{
        //    return transition_list;
        //}
        //public void setTransition(Transition obj)
        //{
        //    transition_list.Add(obj);
        //}

    }

    public class Transition
    {
        byte from, to;
        string read;

        public Transition(byte _from, byte _to, string _read)
        {
            from = _from;
            to = _to;
            read = _read;
        }
        public string To_State_name { get; set; }
        public byte getFrom()
        {
            return from;
        }
        public byte getTo()
        {
            return to;
        }
        public string getRead()
        {
            return read;
        }
        public void setFrom(byte _f)
        {
            from = _f;
        }
        public void setTo(byte _to)
        {
            to = _to;
        }
        public void setRead(string _read)
        {
            read = _read;
        }

    }

    public class array_node
    {
        //  State_node state1;
        public State_node state1 { get; set; }
        public State_node state2 { get; set; }
        public bool son_distinguibles { get; set; }
        public bool son_indistinguibles { get; set; }
        public int mark { get; set; }

    }

    public class State_list
    {
        List<State_node> Mylist = new List<State_node>();
        List<State_node> Minimized_list = new List<State_node>();
        List<Transition> transition_list = new List<Transition>();

        public int getStatelistsize()
        {
            return Mylist.Count;
        }

        public State_list(string file)
        {
            fill_list(file);
            destroy_all_unreachable_states();
            pass_transition_to_state_node();
            fill_To_state_name();
        }

        public void fill_list(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);


            foreach (XmlNode node in doc.DocumentElement)
            {
                if (node.Name == "automaton")
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        byte id = 0;
                        string name = "";
                        float x_pos = 0, y_pos = 0;
                        bool initial = false, final = false;


                        if (child.Name == "state")
                        {
                            byte.TryParse(child.Attributes[0].InnerText, out id);//id
                            name = child.Attributes[1].InnerText;//name

                            foreach (XmlNode state_child in child.ChildNodes)
                            {
                                if (state_child.Name == "x")
                                    float.TryParse(state_child.InnerText, out x_pos);

                                if (state_child.Name == "y")
                                    float.TryParse(state_child.InnerText, out y_pos);

                                if (state_child.Name == "initial")
                                    initial = true;

                                if (state_child.Name == "final")
                                    final = true;
                            }

                            State_node _node = new State_node(id, name, x_pos, y_pos, initial, final);
                            Mylist.Add(_node);

                        }
                        if (child.Name == "transition")
                        {
                            byte from = 0, to = 0;
                            string read = "";
                            foreach (XmlNode state_child in child.ChildNodes)
                            {

                                if (state_child.Name == "from")
                                    byte.TryParse(state_child.InnerText, out from);

                                if (state_child.Name == "to")
                                    byte.TryParse(state_child.InnerText, out to);

                                if (state_child.Name == "read")
                                    read = state_child.InnerText;
                            }

                            Transition state_transition = new Transition(from, to, read);

                            transition_list.Add(state_transition);
                        }


                    }


                }



            }

        }

        public void destroy_all_unreachable_states()
        {
            foreach (State_node item in Mylist.ToList())
            {
                bool is_unreachable = true;

                for (byte i = 0; i < transition_list.Count; i++)
                {
                    if (item.getInit() == true)
                    {
                        is_unreachable = false;
                        break;
                    }
                    else if (transition_list[i].getTo() == item.getId())//si hay un to con ese numero
                    {
                        is_unreachable = false;
                        break;
                    }
                }

                if (is_unreachable == true)
                {
                    foreach (Transition tran_item in transition_list.ToList())
                    {
                        if (tran_item.getFrom() == item.getId())
                        {
                            transition_list.Remove(tran_item);
                        }
                    }
                    Mylist.Remove(item);

                }

            }



        }

        public void pass_transition_to_state_node()
        {
            int x = 0;
            foreach (State_node node in Mylist.ToList())
            {

                foreach (Transition item in transition_list)
                {
                    if (node.getId() == item.getFrom())
                    {
                        Mylist[x].get_trans().Add(item);
                    }
                }
                x++;
            }
        }

        public void fill_To_state_name()
        {
            int x = 0;
            foreach (State_node node in Mylist.ToList())
            {
                foreach (State_node item in Mylist)
                {
                    for (int i = 0; i < Mylist[x].get_trans().Count; i++)
                    {
                        if (item.getId() == Mylist[x].get_trans()[i].getTo())
                        {
                            Mylist[x].get_trans()[i].To_State_name = item.getName();
                        }
                    }


                }

                x++;
            }
        }

        public void print()
        {
            int i = 0;
            Console.WriteLine("//States");
            foreach (State_node item in Mylist)
            {

                Console.Write("ID:" + item.getId() + '\n' + "Name:" + item.getName() + '\n' + "x_pos:" + item.getX()
                + '\n' + "y_pos:" + item.getY() + '\n' + "Initial:" + item.getInit() + '\n' + "Final:" + item.getFinal());
                for (int x = 0; x < Mylist[i].get_trans().Count; x++)
                {

                    Console.Write('\n' + "From:" + item.get_trans()[x].getFrom() + '\n' + "Read:" + item.get_trans()[x].getRead() + '\n' + "To:" + item.get_trans()[x].getTo()
                        + '\n' + "To State Name:" + item.get_trans()[x].To_State_name);

                    Console.WriteLine('\n');
                }
                Console.WriteLine();
                Console.WriteLine('\n');
                i++;

            }

            //print transitions
            //Console.WriteLine("//Transitions");
            //foreach (Transition item in transition_list)
            //{

            //    Console.WriteLine("From:" + item.getFrom() + '\n' + "To:" + item.getTo() + '\n' + "Read:" + item.getRead());
            //}




        }


        public void print_minimized_list()
        {
            int i = 0;
            Console.WriteLine("//States");
            foreach (State_node item in Minimized_list)
            {

                Console.Write("ID:" + item.getId() + '\n' + "Name:" + item.getName() + '\n' + "x_pos:" + item.getX()
                + '\n' + "y_pos:" + item.getY() + '\n' + "Initial:" + item.getInit() + '\n' + "Final:" + item.getFinal());
                for (int x = 0; x < Minimized_list[i].get_trans().Count; x++)
                {

                    Console.Write('\n' + "From:" + item.get_trans()[x].getFrom() + '\n' + "Read:" + item.get_trans()[x].getRead() + '\n' + "To:" + item.get_trans()[x].getTo()
                        + '\n' + "To State Name:" + item.get_trans()[x].To_State_name);

                    Console.WriteLine('\n');
                }
                Console.WriteLine();
                Console.WriteLine('\n');
                i++;

            }

            //print transitions
            //Console.WriteLine("//Transitions");
            //foreach (Transition item in transition_list)
            //{

            //    Console.WriteLine("From:" + item.getFrom() + '\n' + "To:" + item.getTo() + '\n' + "Read:" + item.getRead());
            //}




        }

        public bool isAFD(List<State_node> Mylist)
        {

            int state1_num_transitions = 0;
            int state2_num_transitions = 0;
            //state 1

            state1_num_transitions = Mylist[0].get_trans().Count;
           // Ver si hay alguno que no tien la misma cnatidad de estados
            foreach (State_node node in Mylist)
            {
                state2_num_transitions = node.get_trans().Count;

                if (state1_num_transitions != state2_num_transitions)
                    return false;

            }


            int cont=0;
            foreach (State_node node in Mylist)
            {
                //Por cada nodo
                for (int x = 0; x < Mylist.Count; x++)
                {
                    //Por cada transicion del nodo
                    for (int i = 0; i < Mylist[x].get_trans().Count; i++)
                    {
                        cont = 0;
                        //Por cada transicion del nodo para leer el read
                        for (int j = 0; j < Mylist[x].get_trans().Count; j++)
                        {
                            if(Mylist[x].get_trans()[i].getRead()== Mylist[x].get_trans()[j].getRead())
                            {
                                cont++;
                                if (cont > 1)
                                {
                                    return false;
                                }
                                
                            }
                        }
                    }
                }
               
            }
                return true;
        }


        public void print_mark(List<array_node> Pair_states_list)
        {
            //Print mark
            foreach (array_node pair in Pair_states_list)
            {

                Console.WriteLine("Par: " + pair.state1.getName() + "," + pair.state2.getName() + " Mark:" + pair.mark);

            }
        }

        public void reduction_algorithim()
        {
            if (isAFD(Mylist) == false)
            {
                MessageBox.Show("It's not AFD, please enter an AFD");
                return;

            }
            State_node[] vertical_states_arr = new State_node[Mylist.Count - 1];
            State_node[] horizontal_states_arr = new State_node[Mylist.Count - 1];
            List<array_node> Pair_states_list = new List<array_node>();
            bool has_distinguibles = false;
            int cont = 0;

            //Lenar los dos arreglos unidimensionales
            foreach (State_node node in Mylist)
            {
                if (cont == 0)
                {
                    horizontal_states_arr[cont] = node;
                }
                if (cont >= 1 && cont <= (Mylist.Count - 1))
                {

                    vertical_states_arr[cont - 1] = node;
                }
                if (cont >= 1 && cont != (Mylist.Count - 1))
                {

                    horizontal_states_arr[cont] = node;
                }

                cont++;

            }
            //Lenar la lista Pair states de onjetos state y marcar los distinguibles
            int contador = 0;
            for (int i = 0; i < horizontal_states_arr.Length; i++)
            {
                for (int j = 0; j < vertical_states_arr.Length; j++)
                {
                    int num = j + contador;
                    if (num < vertical_states_arr.Length)
                    {
                        array_node node = new array_node();
                        node.state1 = horizontal_states_arr[i];
                        node.state2 = vertical_states_arr[num];
                        if (vertical_states_arr[num].getFinal() == true && horizontal_states_arr[i].getFinal() == false ||
                           vertical_states_arr[num].getFinal() == false && horizontal_states_arr[i].getFinal() == true)
                        {
                            node.son_distinguibles = true;
                            node.son_indistinguibles = false;
                            node.mark = 0;
                            has_distinguibles = true;
                        }
                        else
                        {
                            node.son_distinguibles = false;
                            node.son_indistinguibles = true;
                            node.mark = -1;
                        }

                        Pair_states_list.Add(node);
                    }
                }

                contador++;
            }
            if (has_distinguibles == false)
            {
                MessageBox.Show("This AFD can't be minimized");
                return;
            }

            //Algoritmo para marcar
            int to_s1;
            int to_s2;
            bool parar = false;
            while (parar != true)
            {
                contador = 0;
                parar = true;
                foreach (array_node Pair_item in Pair_states_list.ToList())
                {

                    //Si el par es indisitnguible(que tiene un espacio libre) y estam arcado con -1
                    if (Pair_item.mark == -1)
                    {
                        //Buscar las transiciones de el estado 1 y el estado 2
                        for (int i = 0; i < Pair_item.state1.get_trans().Count; i++)
                        {
                            for (int j = 0; j < Pair_item.state2.get_trans().Count; j++)
                            {

                                //Si estan leyendo la misma variable, buscar los To
                                if (Pair_item.state1.get_trans()[i].getRead() == Pair_item.state2.get_trans()[j].getRead())
                                {
                                    //Buscar los dos estados que dieron de reulstado en el To
                                    foreach (array_node node in Pair_states_list)
                                    {
                                        //Si el nuevvo par de TO es un par de los que esta en la lista de pares 
                                        to_s1 = Pair_item.state1.get_trans()[i].getTo();
                                        to_s2 = Pair_item.state2.get_trans()[j].getTo();
                                        if (to_s2 == to_s1)
                                        {
                                            break;
                                        }

                                        if (Pair_item.state1.get_trans()[i].getTo() == node.state1.getId() &&
                                            Pair_item.state2.get_trans()[j].getTo() == node.state2.getId())
                                        {
                                            if (node.mark >= 0)
                                            {
                                                Pair_states_list[contador].mark = node.mark + 1;
                                                parar = false;

                                            }
                                            break;
                                        }
                                        else if (Pair_item.state1.get_trans()[i].getTo() == node.state2.getId() &&
                                             Pair_item.state2.get_trans()[j].getTo() == node.state1.getId())
                                        {
                                            if (node.mark >= 0)
                                            {
                                                Pair_states_list[contador].mark = node.mark + 1;
                                                parar = false;
                                            }
                                            break;

                                        }


                                    }


                                }


                            }

                        }

                    }

                    contador++;
                }

            }

            //Print Mark
            print_mark(Pair_states_list);


            //Create new states formed 
            byte id = 0;
            string name = "";
            float x = 0;
            float y = 0;
            bool initial = false;
            bool final = false;
            List<State_node> temp_list = Mylist;

            foreach (array_node Pair_item in Pair_states_list)
            {
                initial = false;
                final = false;
                //Si el par es equivalente
                if (Pair_item.mark == -1)
                {

                    //Se concatena el nombre
                    name = Pair_item.state1.getName() + "," + Pair_item.state2.getName();
                    x = Pair_item.state1.getX();
                    y = Pair_item.state1.getY();
                    //Si alguno del par es inicial
                    if (Pair_item.state1.getInit() == true || Pair_item.state2.getInit() == true)
                    {
                        initial = true;
                    }
                    //Si alguno del par es final
                    if (Pair_item.state1.getFinal() == true || Pair_item.state2.getFinal() == true)
                    {
                        final = true;
                    }

                    State_node node = new State_node(id, name, x, y, initial, final);
                    foreach (Transition tran in Pair_item.state1.get_trans())
                    {
                        tran.setFrom(id);
                        if (tran.To_State_name == Pair_item.state1.getName() || tran.To_State_name == Pair_item.state2.getName())
                        {
                            tran.To_State_name = name;
                        }
                        node.get_trans().Add(tran);
                    }

                    //Se agrega el nuevo nodo a una lista
                    Minimized_list.Add(node);
                    //Borro los nodos separados que ahora estan unidos de la lista temporal
                    foreach (State_node item in temp_list.ToList())
                    {
                        if (item.getName() == Pair_item.state1.getName() || item.getName() == Pair_item.state2.getName())
                        {
                            temp_list.Remove(item);
                        }
                    }
                    int numerito = 0;
                    foreach (State_node node_in_list in temp_list.ToList())
                    {

                        for (int i = 0; i < node_in_list.get_trans().Count; i++)
                        {
                            if (node_in_list.get_trans()[i].To_State_name == Pair_item.state1.getName())
                            {
                                //chaka
                                temp_list[numerito].get_trans()[i].To_State_name = name;
                            }
                            if (node_in_list.get_trans()[i].To_State_name == Pair_item.state2.getName())
                            {
                                temp_list[numerito].get_trans()[i].To_State_name = name;
                            }
                        }

                        numerito++;

                    }


                    id++;
                }
                else
                {
                    continue;
                }
            }

            //Agregar los nodos restantes, los que estan separados
            foreach (State_node node_from_temp_list in temp_list)
            {
                //Agregar nuevo id a los nodos separados
                node_from_temp_list.setId(id);
                id++;
                Minimized_list.Add(node_from_temp_list);
            }

            //Agregar las transiciones a los nodos

            foreach (State_node node_in_minimized_list in Minimized_list)
            {
                foreach (Transition node_transition in node_in_minimized_list.get_trans())
                {
                    //Ponerle el nuevo From a los nodos, que tiene que cambiar porque su id cambio
                    node_transition.setFrom(node_in_minimized_list.getId());

                    foreach (State_node node_searching in Minimized_list)
                    {
                        if (node_transition.To_State_name == node_searching.getName())
                        {
                            node_transition.setTo(node_searching.getId());
                        }
                    }

                }

            }

            MessageBox.Show("Automaton has been minimized");




        }
        public void write_Xml(string file_path)
        {
            string nam = "C:\\Users\\sanch\\Desktop\\JFF_Files\\Minimized_examples\\" + Path.GetFileName(file_path);
            XmlWriter xmlWriter = XmlWriter.Create(nam);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("structure");

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("fa");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("automaton");

            string id;
            string name;
            string x;
            string y;
            string from;
            string read;
            string to;
            foreach (State_node node in Minimized_list)
            {
                id = node.getId().ToString();
                name = node.getName();
                x = node.getX().ToString();
                y = node.getX().ToString();

                //start of state
                xmlWriter.WriteStartElement("state");
                xmlWriter.WriteAttributeString("id", id);
                xmlWriter.WriteAttributeString("name", name);

                xmlWriter.WriteStartElement("x");
                xmlWriter.WriteString(x);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("y");
                xmlWriter.WriteString(y);
                xmlWriter.WriteEndElement();

                if (node.getInit() == true)
                {
                    xmlWriter.WriteStartElement("initial");
                    xmlWriter.WriteEndElement();
                }
                if (node.getFinal() == true)
                {
                    xmlWriter.WriteStartElement("final");
                    xmlWriter.WriteEndElement();
                }
                //end of state
                xmlWriter.WriteEndElement();
            }
            foreach (State_node node in Minimized_list)
            {
                foreach (Transition tran in node.get_trans())
                {
                    from = tran.getFrom().ToString();
                    read = tran.getRead();
                    to = tran.getTo().ToString();

                    xmlWriter.WriteStartElement("transition");

                    xmlWriter.WriteStartElement("from");
                    xmlWriter.WriteString(from);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("to");
                    xmlWriter.WriteString(to);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("read");
                    xmlWriter.WriteString(read);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();

                }
            }


            xmlWriter.WriteEndElement();
            // xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            //xmlWriter.WriteStartElement("user");
            //xmlWriter.WriteAttributeString("age", "42");
            //xmlWriter.WriteString("John Doe");
            //xmlWriter.WriteEndElement();

            //xmlWriter.WriteStartElement("user");
            //xmlWriter.WriteAttributeString("age", "39");
            //xmlWriter.WriteString("Jane Doe");


        }


    }



}





