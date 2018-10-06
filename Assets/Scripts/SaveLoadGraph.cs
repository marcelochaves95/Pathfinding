using UnityEngine;
using System.Xml;
using System.IO;

public class SaveLoadGraph : MonoBehaviour {

	public struct Data {
		public int example { get; set; }
        public int[] exampleVet { get; set; }
    }

	public Data data;

	public void SaveGraph (string name) {
		Debug.Log("Start saving...");
		
		/// <summary>
		/// Arquivos que serão salvos aqui
		/// </summary>
		//data.example = 
		//data.exampleVet = 
		
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

    public void LoadGraph (string name) {
        try {
            int invCount = 0;
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
                            int auxStack = reader.ReadElementContentAsInt();
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