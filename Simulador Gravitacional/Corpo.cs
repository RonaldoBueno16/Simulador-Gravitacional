using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_Gravitacional
{
    class Corpo
    {
        private string nome;
        private double massa;
        private double raio;
        private double posX;
        private double posY;
        private double velX;
        private double velY;

        private double force;
        private double forceX;
        private double forceY;

        public Corpo(string nome_, double massa_, double raio_, double posX_, double posY_, double velX_, double velY_)
        {
            this.nome = nome_;
            this.massa = massa_;
            this.raio = raio_;
            this.posX = posX_;
            this.posY = posY_;
            this.velX = velX_;
            this.velY = velY_;
            this.ClearForce();
        }

        public void ClearForce()
        {
            this.force = 0.0;
            this.forceX = 0.0;
            this.forceY = 0.0;
        }

        public double getForce()
        {
            return this.force;
        }

        public double getForceX()
        {
            return this.forceX;
        }

        public double getForceY()
        {
            return this.forceY;
        }

        public void addForce(double value, double valueX, double valueY)
        {
            this.force += value;
            this.forceX += valueX;
            this.forceY += valueY;
        }

        public void setPosition(double newX, double newY)
        {
            this.posX = newX;
            this.posY = newY;
        }

        public void setVelocity(double newX, double newY)
        {
            this.velX = newX;
            this.velY = newY;
        }

        public string getName()
        {
            return this.nome;
        }

        public double getMassa()
        {
            return this.massa;
        }

        public double getRaio()
        {
            return this.raio;
        }

        public double getPositionX()
        {
            return this.posX;
        }

        public double getPositionY()
        {
            return this.posY;
        }

        public double getVelocityX()
        {
            return this.velX;
        }

        public double getVelocityY()
        {
            return this.velY;
        }

        public double getDistance(Corpo other_body)
        {
            return Math.Sqrt(Math.Pow((this.getPositionX() - other_body.getPositionX()), 2) + Math.Pow((this.getPositionY() - other_body.getPositionY()), 2));
        }

        public override string ToString()
        {
            return String.Format("{0};{1};{2:0.######};{3:0.######};{4:0.######};{5:0.######};{6:0.######}", this.nome, this.massa, this.raio, this.posX, this.posY, this.velX, this.velY);
        }
    }
}
