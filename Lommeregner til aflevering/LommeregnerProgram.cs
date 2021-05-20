using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lommeregner_til_aflevering
{
    static class LommeregnerProgram
    {
        [STAThread]
        static void Main()
        {
            //Disse tre metodekald er en del namesppacet System.Windows.Forms og er krævet for at programmet kan åbnes som et vindue
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LommeregnerForm());
            //Mere er ikke krævet i Main, da resten af programmet kaldes ved events i brugergrænsefladen.
        }
    }
    //Dette er den class, der indeholder alt koden til at Parse regnestykket fra tekst til data-træet
    public static class CalculatorParser
    {
        //Returnere en string som er værdien af regnestykket.
        public static string ValueAsString(string equation, out bool succesful)
        {
            return ValueAsDouble(equation, out succesful).ToString();
        }


        //Returnere en double som er værdien af regnestykket
        public static double ValueAsDouble(string equation, out bool succesful)
        {
            if (equation.Trim() == string.Empty) //Tjekker om regnestykket er tomt eller udelukkende består af mellemrum
            {
                succesful = false;
                return double.NaN;
            }
            FormatString(ref equation); //Formaterer regnestykket ift. til teksten. Forbereder stringen på at blive læst af resten af programmet. "equation" bruges med ref keywordet da ændringer foretaget i FormatString derved bibeholdes.
            //Laver en Operator som indeholder alle de andre operators i træet
            Operator op = CreateTree(equation);
            if (op != null) //Hvis ikke denne Operator er null, er regnestykket oversat succesfuldt og værdien af Operatoren kan findes.
            {
                succesful = true;
                return op.GetValue();
            }
            //Hvis Operatoren er null, er regnestykket ikke oversat succesfuldt. 0 returneres
            succesful = false;
            return 0;
        }


        //Laver ud fra det givne regnestykke og returnere den øverste Operator i dette træ.
        private static Operator CreateTree(string equation)
        {
            //Hvis regnestykket kan splittes mere op, gøres dette med den Aritmetiske type som sendes ud af SplitUpMore med out keywordet.
            //"1+2*3+4" kan f.eks. splittes op i "1" og "2*3" og "4" ved den aritmetiske type Add
            if (SplitUpMore(equation, out Arithmetics splitType))
                return CreateChain(Split(equation, splitType), splitType); //retunere en Operator der bruger den Aritmetiske type på alle ledene sammen. Sagt mindre krøllet hvis "1+2+3" sendes vil en operator, der i sidste ende lægger tallene sammen returnes.
            else return null;
        }


        //Formatere og forbereder regnestykket som string til at blive omsat et træ. ref bruges fordi jeg er doven og ikke gider skrive "equation = FormatString(equation);" men hellere vil skrive "FormatString(equation);"
        private static void FormatString(ref string equation)
        {
            //Mellemrum fjernes og "--" erstattes med "+". "1 -- 2" bliver til "1+2"
            equation = equation.Replace(" ", string.Empty).Replace("--", "+").Replace(")(", ")*(");

            //hvis (Første tegn er '(' eller første to tegn er "-("   eller regnestykket udelukkende består af et tal
            //if (equation[0] == '(' || equation.Substring(0, 2) == "-(" || double.TryParse(equation, out _))
                //equation = equation.Insert(0, "0+"); //Indsææter "0+" først i regnestykket. Fixer problemet med fejl, hvis der overskud af parenteser, negative parenteser først eller hvis der kun skrives et tal som regnestykke

            //Indsætter gange tegn mellem tal og parenteser f.eks. "2(3+4)" bliver til "2*(3+4)" og mellem tal og bogstaver f.eks. "2pi" bliver til "2*pi"
            for (int i = 0; i < equation.Length - 1; i++)
            {
                if (char.IsDigit(equation[i]) && equation[i + 1] == '(' || char.IsDigit(equation[i]) && char.IsLetter(equation[i + 1])) //Placing multiplication symbols between numbers and parentheses ( "6(2+2)" -> "6*(2+2)" ) and between numbers and letters
                    equation = equation.Insert(i + 1, "*");
            }
            equation = equation.Replace("pi", Math.PI.ToString()).Replace("e", Math.E.ToString());
        }


        //Returnere en Operator der bruger den aritmetiske type på hvert af ledene. Gives {"1", "2*3", "4/5"} og Add som type, returneres en operator hvis GetValue() retunere værdien af de tre led adderet. 
        public static Operator CreateChain(string[] links, Arithmetics type)
        {
            //Da der som minimum kræver to led kastes en fejl, hvis der er mindre en to led
            if (links.Length < 2)
                throw new ArgumentOutOfRangeException("links", "Cannot create chain with less than two links");
            Operator[] ops = new Operator[links.Length - 1]; //Der kræves (antal led - 1) Operatorer
            for (int i = 0; i < links.Length - 1; i++)//Kører hver enkelt led igennem og tilføjer det til kæden
            {
                //Først defineres det hvilken aritmetisk type der bruges. Dette kan gøres da Add, Minus osv. inheritor fra den abstracte class Operator
                Operator op;
                if (type == Arithmetics.Add)
                    op = new Add();
                else if (type == Arithmetics.Minus)
                    op = new Minus();
                else if (type == Arithmetics.Multi)
                    op = new Multi();
                else if (type == Arithmetics.Divi)
                    op = new Divi();
                else
                    op = new Power();

                //Håndtering af første operator i kæden
                if (i == 0) //Speciel start da vi ved den første har et udtryk og ikke en tidligere lavet del af kæden
                {
                    if (ParentesCheck(ref links[i], out bool toBeInverted))  //Håndtering af parenteser. Hvis parenteses er negativ er "toBeInverted" true
                        op.SetOne(CreateTree(links[i]).Invert(toBeInverted));
                    else if (SplitUpMore(links[i /* 0 */], out Arithmetics splitType)) //Håndtering af udtryk, der skal splittes mere op.
                        op.SetOne(CreateChain(Split(links[0], splitType), splitType));
                    else
                        op.SetOne(double.Parse(links[i])); //Hvis dette led uddelukkende består af et tal, bliver dette indsat
                }
                else //Hvis ikke det er første operator i kæden skal den tidligere operator bare sættes som "OperatorOne" i den nuværende Operator.
                    op.SetOne(ops[i - 1]);

                //Håndtering af anden operand i operatoren. Dettte er det samme som før.
                if (ParentesCheck(ref links[i + 1], out bool toBeInvertedTwo))
                    op.SetTwo(CreateTree(links[i + 1]).Invert(toBeInvertedTwo));
                else if (SplitUpMore(links[i + 1], out Arithmetics splitTypeTwo))
                    op.SetTwo(CreateChain(Split(links[i + 1], splitTypeTwo), splitTypeTwo));
                else
                    op.SetTwo(double.Parse(links[i + 1]));

                ops[i] = op;
            }
            return ops[ops.Length - 1]; //Returnere den sidste operator i "ops". Dette gøres da den sidste er i toppen af det opbyggede træ.
        }


        //Returnere om "equation" starter og slutter med parenteser og om det første parentes er negativ. Parenteser og minus fjernes, hvis de er der, hvilket er hvorfor "equation" kaldes med ref keywordet.
        //Derudover er "isNegative" med out keywordet, da den indikerer om det er en negativ parentes.
        public static bool ParentesCheck(ref string equation, out bool isNegative)
        {
            isNegative = false;
            if (equation[equation.Length - 1] == ')') //stringen ender på ')'
            {
                if (equation[0] == '(')
                {
                    equation = equation.Substring(1, equation.Length - 2); //Fjerner parenteserne i hver ende
                    return true;
                }
                if (equation.Substring(0, 2) == "-(")
                {
                    isNegative = true;
                    equation = equation.Substring(2, equation.Length - 3); //Fjerner parenteser samt start-minus i hver ende
                    return true;
                }
            }
            return false;
        }


        //Denne dikterer rækkefølgen af hvornår operatorene bliver ledt efter. Jo længere nede desto senere og derved højere opppe i regnehiarkiet.
        //Der tjekkes om regnestykket kan splittes mere op ved hver aritmetiske type og returnere den type, der kan splittes ved.


        private static bool SplitUpMore(string equation, out Arithmetics splitType)
        {
            if (Split(equation, Arithmetics.Add).Length >= 2)
            {
                splitType = Arithmetics.Add;
                return true;
            }
            if (Split(equation, Arithmetics.Minus).Length >= 2)
            {
                splitType = Arithmetics.Minus;
                return true;
            }
            if (Split(equation, Arithmetics.Multi).Length >= 2)
            {
                splitType = Arithmetics.Multi;
                return true;
            }
            if (Split(equation, Arithmetics.Divi).Length >= 2)
            {
                splitType = Arithmetics.Divi;
                return true;
            }
            if (Split(equation, Arithmetics.Power).Length >= 2)
            {
                splitType = Arithmetics.Power;
                return true;
            }
            splitType = Arithmetics.None;
            return false;
        }


        //Splitter "equation" ved den givne aritmetiske type.
        public static string[] Split(string equation, Arithmetics type)
        {
            //Først ærkleres og herefter defineres, hvilken karakter der skal splittes ved.
            char splitChar = '?';
            switch (type)
            {
                case Arithmetics.Add:
                    splitChar = '+';
                    break;
                case Arithmetics.Minus:
                    splitChar = '-';
                    break;
                case Arithmetics.Multi:
                    splitChar = '*';
                    break;
                case Arithmetics.Divi:
                    splitChar = '/';
                    break;
                case Arithmetics.Power:
                    splitChar = '^';
                    break;
            }

            var links = new List<string>();
            int lastSplitIndex = 0; //Det sidste index hvor der blev splittet
            int parentesCount = 0; //Antal parenteser "inde" i der er ved det nuværende index. Bruges da der ikke skal splittes inden i parenteser, da det skal regnes som én samlet værdi
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i] == '(')
                    parentesCount++;
                else if (equation[i] == ')')
                    parentesCount--;
                else if (i > 0) //Der ledes efter operatorer og da de aldrig vil være forrest, er der ingen grund til at tjekke
                {
                    if (equation[i] == splitChar && parentesCount == 0) //Hvis karakteren på indexet er lig med den karakter der splittes ved og man ved indexet ikke er inde i nogen parenteser.
                    {
                        //Special case hvis det er minus, for at ungå at "2*-3" bliver tolket som om vi prøver at trække 3 fra noget
                        if (!(splitChar == '-' && CharIsAnyOfFollowing(equation[i - 1], '*', '/', '^')))
                            links.Add(equation.Substring(lastSplitIndex, i - lastSplitIndex));//Tilføjer ledet til "links"
                        lastSplitIndex = i + 1;
                    }
                }
            }
            links.Add(equation.Substring(lastSplitIndex)); //Tilføjer det sidste led
            if (parentesCount != 0)
                throw new Exception("Mismatched brackets"); //Hvis ikke denne er 0 er der ikke lige mange start- som slutparenteser
            return links.ToArray();
        }


        //Returnere om "charToCkeck" er lig med nogen af "chars"
        private static bool CharIsAnyOfFollowing(char CharToCheck, params char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (CharToCheck == chars[i])
                    return true;
            }
            return false;
        }


        //De aritmetiske typer. Bruges for at gøre koden mere læsbar.
        public enum Arithmetics
        {
            Add,
            Minus,
            Multi,
            Divi,
            Power,
            None
        }
    }

    //Dette er den class som alle de implementerede operatorer inheritor fra. Da Operator aldrig skal kunne instantieres bruges abstract keywordet.
    //Dette gøres da den eneste forskel på operatorene er hvordan værdien udregnes.
    public abstract class Operator
    {
        //Da opetorene både kan indeholde tal direkte eller andre operatorer, der returnere tal, er der både to Operators og to doubles "2+2"
        private Operator OperatorOne;
        private Operator OperatorTwo;
        private double ValueOne;
        private double ValueTwo;

        //Sættes til true, hvis værdien som operatoren skal returnere skal ganges med -1. Gøres ved negative parenteser 
        private bool Inverted = false;

        //Returnere værdien af det der ville svare tol udtrykket til venstre for operatoren.
        protected double GetOne()
        {
            if (OperatorOne != null) //Hvis operaoten i OperatorOne ikke er null, kaldes OperatorOnes GetValue(). 
                return OperatorOne.GetValue();
            return ValueOne; //Ellers returneres ValueOne
        }
        protected double GetTwo()
        {//Se GetOne for forklaring
            if (OperatorTwo != null)
                return OperatorTwo.GetValue();
            return ValueTwo;
        }

        //Til at sætte værdien af Value- og Operator- One og Two bruges overloading. Dette er nødvendigt, men gør det nemmere
        public void SetOne(Operator op)
        {
            OperatorOne = op;
        }
        public void SetOne(double op)
        {
            ValueOne = op;
        }
        public void SetTwo(Operator op)
        {
            OperatorTwo = op;
        }
        public void SetTwo(double op)
        {
            ValueTwo = op;
        }

        //Metode til at "inverte" operatoren. Ganger resultatet med -1.  "2*-(2+2)
        public Operator Invert(bool toInvert)
        {
            if (toInvert)
                Inverted = !Inverted;
            return this;
        }

        //Returnere -1, hvis operatoren er inverted og 1 ellers
        protected int GetInverted()
        {
            if (Inverted)
                return -1;
            return 1;
        }
        //Denne returnerer værdien som double af operatoren. Er abstract da hver operator-class udregner værdien på forskellige måder. (Add plusser, Multi ganger osv.)
        //Ved at have den abstract skal en derived class have en implementation af GetValue()
        public abstract double GetValue();
    }

    //Klasserne der håndterer hver operator
    public class Add : Operator
    {
        public override double GetValue()
        {
            return (GetOne() + GetTwo()) * GetInverted();
        }
    }
    public class Minus : Operator
    {
        public override double GetValue()
        {
            return (GetOne() - GetTwo()) * GetInverted();
        }
    }
    public class Multi : Operator
    {
        public override double GetValue()
        {
            return GetOne() * GetTwo() * GetInverted();
        }
    }
    public class Divi : Operator
    {
        public override double GetValue()
        {
            return GetOne() / GetTwo() * GetInverted();
        }
    }
    public class Power : Operator
    {
        public override double GetValue()
        {
            return Math.Pow(GetOne(), GetTwo()) * GetInverted();
        }
    }
}