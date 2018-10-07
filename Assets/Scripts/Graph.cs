using UnityEngine;
using System.Xml;
using System.Collections.Generic;

public class Graph : MonoBehaviour {

    public struct Data {
		public int example { get; set; }
        public int[] exampleVet { get; set; }
        public bool status;
        public Vector3 pos;
        public Neighbor node;
        public List<Neighbor> connectedNodes;
    }

	public Data data;

    public bool status;
    public Vector3 position;
    public Neighbor node;
    public List<Neighbor> connectedNodes;

    public Graph (bool status, Vector3 position, List<Neighbor> connectedNodes) {
        this.status = status;
        this.position = position;
        this.connectedNodes = connectedNodes;
    }

    public void SaveGraph () {
		Debug.Log("Start saving...");

		/// <summary>
		/// Arquivos que serão salvos aqui
		/// </summary>
		
        XmlWriterSettings settings = new XmlWriterSettings {
            Indent = true
        };
        XmlWriter writer = XmlWriter.Create(name + @".xml", settings);
		writer.WriteStartDocument();
		writer.WriteStartElement("Nome da variável que será");
		writer.WriteString(data.example.ToString());
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteStartElement("Nome da variável que será");
		for (int i = 0; i < data.exampleVet.Length; i++) {
			writer.WriteStartElement("Nome da variável que será");
			writer.WriteString(data.exampleVet[i].ToString());
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
		writer.Flush();
		writer.Close();
        Debug.Log("Grapf saved...");
	}

    public void LoadGraph () {
        try {
            //int invCount = 0;
            int staCount = 0;

            XmlReader reader = XmlReader.Create(name + @".xml");
            while (reader.Read()) {
                if (reader.IsStartElement()) {
                    switch (reader.LocalName) {
                        case "Nome da variável que será":
                            Debug.Log("Reading XML");
                            do {
                                reader.Read();
                            } while (reader.Name.Equals("Whitespace"));
                            break;
                        case "Nome da variável que será 2":
                            do {
                                reader.Read();
                            } while (reader.Name.Equals("Whitespace"));
                            break;
                        case "Nome da variável que será 2.1":
                            Debug.Log("Reading XML (internal)");
                            //int auxStack = reader.ReadElementContentAsInt();
                            // Variável que vem da classe Graph[staCount] = auxStack;
                            staCount++;
                            Debug.Log("Read XML");
                            break;
						}
					}
            	}
            reader.Close();
            Debug.Log("Graph loaded...");
        } catch (UnityException) {
            Debug.Log("Save file corrupted!");
        }
    }
}