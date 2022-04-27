using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Simulador_Gravitacional
{
    class Universo
    {
        private int qtd_corpos;
        private int interacoes;
        private int tempo;
        private double G = (6.674184 * Math.Pow(10, -11));
        private List<string> output_list = new List<string>();
        List<Corpo> corpos = new List<Corpo>();

        public Universo()
        {
            this.ReadBody(); //Lê o arquivo de entrada

            output_list.Add(String.Format("{0};{1}", corpos.Count, interacoes));

            Console.WriteLine("Interações: " + interacoes);

            for (int i = 0; i < interacoes; i++)
            {
                output_list.Add(String.Format("** Interacao {0} ************", i));
                foreach (Corpo corpo_1 in corpos)
                {
                    //Calcular força de todos os corpos
                    if(i > 0)
                    {
                        foreach (Corpo corpo_2 in corpos)
                        {
                            if (corpo_1 == corpo_2) continue;

                            CalculateForce(corpo_1, corpo_2); //Acumula a força dos corpos
                        }

                        MoveBody(); //Aplica força nos corpos
                    }
                    output_list.Add(String.Format("{0}", corpo_1.ToString()));
                }

                ApplyColision(); //Aplica a colisão em todos os corpos
                ClearForce(); //Limpar todas as forças
            }

            string file = "output.uni";

            FileStream myFile = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(myFile, Encoding.UTF8);
            
            foreach (var item in output_list)
            {
                sw.WriteLine(item);
            }

            sw.Close();
            myFile.Close();
        }
        
        private void ApplyColision()
        {
            List<Corpo> corpos_removidos = new List<Corpo>();
            foreach(Corpo corpo in corpos)
            {
                foreach(Corpo corpo2 in corpos)
                {
                    if (corpo == corpo2) continue;

                    if(WasColision(corpo, corpo2))
                    {
                        if (corpo.getRaio() > corpo2.getRaio())
                            corpos_removidos.Add(corpo2);
                        else
                            corpos_removidos.Add(corpo);
                    }
                }
            }

            foreach(Corpo corpo in corpos_removidos)
            {
                corpos.Remove(corpo);
            }
        }

        private bool WasColision(Corpo corpo, Corpo corpo2)
        {
            double distance = corpo.getDistance(corpo2);
            if (distance < (corpo.getRaio() + corpo2.getRaio()))
                return true;

            return false;
        }

        private void ClearForce()
        {
            foreach(Corpo corpo in corpos)
            {
                corpo.ClearForce();
            }
        }

        private void MoveBody()
        {
            foreach (Corpo corpo in corpos)
            {
                //Aceleração
                double Ax = corpo.getForceX() / corpo.getMassa();
                double Ay = corpo.getForceY() / corpo.getMassa();

                double velX = corpo.getVelocityX() + (Ax * tempo);
                double velY = corpo.getVelocityY() + (Ay * tempo);

                //S = Sº + Vº * t + ((a * t²)/2)
                double posX = corpo.getPositionX() + (corpo.getVelocityX() * tempo) + (Ax * Math.Pow(tempo, 2)) / 2;
                double posY = corpo.getPositionY() + (corpo.getVelocityY() * tempo) + (Ay * Math.Pow(tempo, 2)) / 2;

                corpo.setPosition(posX, posY);
                corpo.setVelocity(velX, velY);
            }
        }

        private void CalculateForce(Corpo corpo_1, Corpo corpo_2)
        {
            double m1 = corpo_1.getMassa();
            double m2 = corpo_2.getMassa();

            double distance = corpo_1.getDistance(corpo_2);

            //Decompondo a força para o corpo da interação
            double distX = corpo_1.getPositionX() - corpo_2.getPositionX();
            double distY = corpo_1.getPositionY() - corpo_2.getPositionY();

            double force = G * (m1 * m2) / Math.Pow(distance, 2);
            double forceX = force * (distX / distance);
            double forceY = force * (distY / distance);

            corpo_1.addForce(force * -1, forceX * -1, forceY * -1);
            corpo_2.addForce(force, forceX, forceY);
        }


        private void ReadBody()
        {
            Console.WriteLine("Fazendo a leitura inicial dos corpos celestes");

            string[] lines = File.ReadAllLines(@"corpos.txt");

            this.qtd_corpos = Convert.ToInt32(lines[0].Split(';')[0]);
            this.interacoes = Convert.ToInt32(lines[0].Split(';')[1]);
            this.tempo = Convert.ToInt32(lines[0].Split(';')[2]);

            CreateCelestialBody(lines);
            Console.WriteLine("Corpos criados: " + corpos.Count);
        }

        private void CreateCelestialBody(string[] lines)
        {
            for (int line = 1; line < lines.Length; ++line)
            {
                string nome = lines[line].Split(';')[0];
                double massa = double.Parse(lines[line].Split(';')[1]);
                double raio = double.Parse(lines[line].Split(';')[2]);
                double posX = double.Parse(lines[line].Split(';')[3]);
                double posY = double.Parse(lines[line].Split(';')[4]);
                double velX = double.Parse(lines[line].Split(';')[5]);
                double velY = double.Parse(lines[line].Split(';')[6]);

                corpos.Add(new Corpo(nome, massa, raio, posX, posY, velX, velY));
            }
        }
    }
}
