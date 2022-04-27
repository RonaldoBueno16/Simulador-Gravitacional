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

        /// <summary>
        /// Classe universo
        /// </summary>
        public Universo()
        {
            this.ReadBody(); //Lê o arquivo de entrada

            this.Interacoes();

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

        /// <summary>
        /// Começa as interações
        /// </summary>
        private void Interacoes()
        {
            output_list.Add(String.Format("{0};{1}", corpos.Count, interacoes));

            Console.WriteLine("Interações: " + interacoes);

            for (int i = 0; i < interacoes; i++)
            {
                output_list.Add(String.Format("** Interacao {0} ************", i));
                foreach (Corpo corpo_1 in corpos)
                {
                    //Calcular força de todos os corpos
                    if (i > 0)
                    {
                        foreach (Corpo corpo_2 in corpos)
                        {
                            if (corpo_1 == corpo_2) continue;

                            InteragirCorpo(corpo_1, corpo_2);
                        }

                        MoveBody();
                    }

                    output_list.Add(String.Format("{0}", corpo_1.ToString()));
                }

                CheckColision();
                ClearForce();
            }
        }

        /// <summary>
        /// Checa a colisão de todos os corpos
        /// </summary>
        private void CheckColision()
        {
            List<Corpo> corpos_removidos = new List<Corpo>();
            foreach (Corpo corpo in corpos)
            {
                foreach (Corpo corpo2 in corpos)
                {
                    if (corpo == corpo2) continue;

                    if (WasColision(corpo, corpo2))
                    {
                        if (corpo.getRaio() > corpo2.getRaio())
                            corpos_removidos.Add(corpo2);
                        else
                            corpos_removidos.Add(corpo);
                    }
                }
            }

            foreach (Corpo corpo in corpos_removidos)
                corpos.Remove(corpo);
        }

        /// <summary>
        /// Retorna se há colisão entre os corpos
        /// </summary>
        /// <param name="corpo">Corpo 1</param>
        /// <param name="corpo2">Corpo 2</param>
        /// <returns>Resultado se há colisão</returns>
        private bool WasColision(Corpo corpo, Corpo corpo2)
        {
            double distance = corpo.getDistance(corpo2);
            if (distance < (corpo.getRaio() + corpo2.getRaio()))
                return true;

            return false;
        }

        /// <summary>
        /// Limpa as forças dos corpos
        /// </summary>
        private void ClearForce()
        {
            foreach (Corpo corpo in corpos)
            {
                corpo.ClearForce();
            }
        }

        /// <summary>
        /// Movimenta todos os corpos
        /// </summary>
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

        /// <summary>
        /// Faz a interação de dois corpos calculando a força e decompondo elas
        /// </summary>
        /// <param name="corpo_1">Corpo 1</param>
        /// <param name="corpo_2">Corpo 2</param>
        private void InteragirCorpo(Corpo corpo_1, Corpo corpo_2)
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

        /// <summary>
        /// Realiza a leitura do arquivo de entrada
        /// </summary>
        private void ReadBody()
        {
            Console.WriteLine("Fazendo a leitura inicial dos corpos celestes");

            string[] lines = File.ReadAllLines("corpos.txt");

            this.qtd_corpos = Convert.ToInt32(lines[0].Split(';')[0]);
            this.interacoes = Convert.ToInt32(lines[0].Split(';')[1]);
            this.tempo = Convert.ToInt32(lines[0].Split(';')[2]);

            CreateCelestialBody(lines);
            Console.WriteLine("Corpos criados: " + corpos.Count);
        }

        /// <summary>
        /// Cria um corpo
        /// </summary>
        /// <param name="lines">Array que está os corpos para serem criados</param>
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
