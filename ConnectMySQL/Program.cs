using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;




namespace ConnectMySQL
{
    class MyInventor
    {

        // private string cs = "server=195.199.150.18;user id=tanulo;password=EotvosSzKI;database=mysql;allowuservariables=True;SSL Mode=None";
        private string cs = "server=localhost;user id = root; database = mysql";

        public MyInventor()
        {
            var con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "create database if not exists raktar CHARACTER set = utf8 COLLATE utf8_hungarian_ci;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "use raktar;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE if not exists keszletek(azon INT AUTO_INCREMENT PRIMARY KEY,termek VARCHAR(40),mennyiseg INT,ar INT);";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void bevitel(string name, int value, int price)
        {
            var con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "use raktar";
            cmd.ExecuteNonQuery();
            cmd.CommandText = string.Format("insert into keszletek (termek, mennyiseg, ar) values('{0}', {1}, {2});", name, value, price);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public string PenzLekeres()
        {
            /*decimal osszeg = 0;
            var con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "use raktar";
            cmd.ExecuteNonQuery();
            string sql = "SELECT SUM(mennyiseg*ar) FROM `keszletek`;";
            cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
               osszeg = rdr;
            }
            con.Close();
            rdr.Close();
            return osszeg;*/
            var con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "use raktar";
            cmd.ExecuteNonQuery();
            string sql = "SELECT sum(ar) as Osszertek FROM keszletek;";
            cmd = new MySqlCommand(sql, con);
            string kiir = cmd.ExecuteScalar().ToString();
            return kiir;



        }

        public List<string> lekeres()
        {
            List<string> lekerdezesek = new List<string>();
            var con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "use raktar";
            cmd.ExecuteNonQuery();
            string sql;
            sql = "select * from keszletek;";
            cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                lekerdezesek.Add(rdr[0] + " " + rdr[1] + " " + rdr[2] + " " + rdr[3]);
            }

            con.Close();
            rdr.Close();
            return lekerdezesek;
        }
        public bool Vasarlas(char valasz, int mennyi, ref int ar)
        {
            try
            {
                var con = new MySqlConnection(cs);
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "use raktar";
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("SELECT `mennyiseg` FROM `keszletek` WHERE azon = {0};", valasz);
                string mennyiseg = cmd.ExecuteScalar().ToString();
                if (int.Parse(mennyiseg) > mennyi)
                {
                    cmd.CommandText = string.Format("UPDATE `keszletek` SET mennyiseg = mennyiseg-{0} WHERE `keszletek`.`azon` = {1};", mennyi, valasz);
                    cmd.ExecuteNonQuery();



                    cmd.CommandText = string.Format("SELECT `ar` FROM `keszletek` WHERE azon = {0};", valasz);
                    string ar0 = cmd.ExecuteScalar().ToString();



                    int ar2 = int.Parse(ar0);
                    double osszeg = ar2 * mennyi * 1.2 * 1.27;
                    ar = (int)osszeg;



                    return true;
                }
                else
                {
                    Console.WriteLine("Nincs elég termék készleten!");
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
    }





    class Program
    {
        static void Main(string[] args)
        {
            MyInventor Raktar = new MyInventor();
            //Console.WriteLine("Ügyes!");
            //Raktar.bevitel("Korte", 150, 250);
            //Raktar.lekeres();
            char x;
            do
            {
                Console.WriteLine("*******Üdvözlöm a raktár managment app-ban**********\nVálasszon az alábbiak közül:\n1.Termék bevitele\n2.Termékek kilistázása\n" +
                    "3.Kilépés\n4.Raktárban található termékek összértékének kiírása.\n5.Vásárlás");
                x = Console.ReadKey(true).KeyChar;


                switch (x)
                {
                    case '1':
                        Raktar.bevitel("Korte", 150, 250);
                        break;
                    case '2':
                        List<string> listazz = Raktar.lekeres();
                        foreach (var y in listazz)
                        {
                            Console.WriteLine(y);
                        }
                        break;
                    case '3':
                        break;
                    case '4':
                        string osszPenz = Raktar.PenzLekeres();
                        Console.WriteLine(osszPenz);
                        break;
                    case '5':
                        List<string> listazz2 = Raktar.lekeres();
                        foreach (var y in listazz2)
                        {
                            Console.WriteLine(y);
                        }
                        int ar = 0;
                        Console.WriteLine("Add meg az azonositóját a terméknek:");
                        char valasztas = Console.ReadKey(true).KeyChar;
                        Console.WriteLine("Add meg mennyit akarsz vásárolni: ");
                        int menny = int.Parse(Console.ReadLine());
                        bool szamla = Raktar.Vasarlas(valasztas, menny, ref ar);
                        if (szamla)
                        {
                            Console.WriteLine(ar);
                        }
                        else Console.WriteLine("Tranzakció elutasítva!");
                        break;

                }
            } while (x != '3');

        }
    }
}