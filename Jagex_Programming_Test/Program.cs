using System;
using System.Collections.Generic;

namespace Jagex_Programming_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string line in System.IO.File.ReadLines(@"c:\Users\samiv\Downloads\Jagex Calculator Test\tests.txt"))
            {
                char separator = ':';
                String[] splitLine = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                string testString = splitLine[0];
                string tesOutStr = new string(Calculate(testString));
                Console.WriteLine(testString + "= " + tesOutStr);
            }

            string factorialTest = "4!";
            Console.WriteLine(Calculate(factorialTest));
        }

        static string Calculate(string inputStr)
        {
            //create empty string for return
            string answerStr = string.Empty;

            //format the string so the split function will correctly seperate everything
            string formattedStr = FormatString(inputStr);

            //split up the string into numbers and operators for conversion into postfix
            char separator =  ' ';
            String[] strlist = formattedStr.Split(separator,StringSplitOptions.RemoveEmptyEntries);

            //debug
            foreach (String s in strlist)
            {
                Console.WriteLine(s);
            }

            //convert the string array into a postfix list
            List<string> postfixStrList = new List<string>(ConvertToPostfix(strlist));

            //debug
            foreach (String postStr in postfixStrList)
            {
                Console.WriteLine(postStr);
            }

            //calculate the answer to the equasion now that it is in postfix format
            double numAnswer = CalculateAnswer(postfixStrList);

            //convert the answer to a string then return it
            answerStr = numAnswer.ToString();
            return answerStr;
        }

        //method to format the string so that it can be easily be Split()
        static string FormatString(string str)
        {
            //create empty string for return
            string outputString = string.Empty;

            //create an array of accepted characters to check the expression for
            char[] operatorArray = new char[9] { '^', '*', '/', '+', '-', '(', ')', '.','!' };

            //for each character in the string
            for (int i = 0; i < str.Length; i++)
            {
                //if the character is a number or a space add it to the formatted string
                if (Char.IsDigit(str[i]) || str[i] == ' ')
                {
                    outputString += str[i];
                }
                //if its not a number 
                else
                {
                    //go through the operator array to see if the character matches one of the operators before adding it to the operator list
                    //for error prevention this method should ignore anything that isnt an operator eg letters
                    for (int j = 0; j < operatorArray.Length; j++)
                    {
                        //if it is an operator or decimal add it to the string
                        if (str[i] == operatorArray[j])
                        {
                            //if there isnt a gap between the perentheses or the factorial and the next char add one to make splitting easier
                            if (str[i] == '(' && str[i + 1] != ' ')
                            {
                                outputString += str[i];
                                outputString += ' ';
                            }
                            else if (str[i] == ')' ||  str[i] == '!'  && str[i - 1] != ' ')
                            {
                                outputString += ' ';
                                outputString += str[i];
                            }
                            else
                            {
                                outputString += str[i];
                            }
                        }
                    }
                }

            }
            //return the formatted string
            return outputString;
        }

        //method to assign int priorities to the different calculations that will be used
        static int OperatorPriority(char op)
        {
            switch (op)
            {
                //addition and subtraction have the lowest priority
                case '+':
                case '-':
                    return 1;

                    //then multiplication and division have the second highest priority
                case '*':
                case '/':
                    return 2;

                    //powers 
                case '^':
                    return 3;
                    //factorials
                case '!':
                    return 4;

            }
            return -1;
        }

        //method to rearrange the Split() string from a infix format to a postfix format for easier calculation using a stack
        static List<string> ConvertToPostfix(string[] strArray)
        {
            //create new empty array to hold rearranged results
            List<string> strlist = new List<string>();

            // initializing empty stack
            Stack<string> stack = new Stack<string>();

            //go through the string array 
            //for each string
            foreach (String str in strArray)
            {
                //check the first character
                char c = str[0];
                //if the character is a digit or a negative add the string to the return list
                if (Char.IsDigit(c) || (c == '-' && str.Length > 1))
                {
                    strlist.Add(str);
                }

                //if the first character is an '(', push it to the stack.
                else if (c == '(')
                {
                    stack.Push(str);
                }
                //if the first character is an ')', pop and output from the stack until an '(' is encountered.
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                    {
                        strlist.Add(stack.Pop());
                    }
                    if (stack.Count > 0 && stack.Peek() != "(")
                    {
                        // invalid expression
                        return null; 
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                //if the first character is an operator
                else
                {
                    //while there is something on the stack if the first(checking) characters priority is less
                    //than the priority of the operator on the top of the stack
                    while (stack.Count > 0 && OperatorPriority(c) <= OperatorPriority(stack.Peek()[0]))
                    {
                        //add the top of the stack to the list
                        strlist.Add(stack.Pop());
                    }
                    //otherwise add the operator to the stack to check against next time a operator comes up
                    stack.Push(str);
                }
            }

            // after all of the strings have been checked pop all the the operators from the stack to the string list
            while (stack.Count > 0)
            {
                strlist.Add(stack.Pop());
            }
            //return the string list
            return strlist;
        }

        //method to calculate the answer to the equasion using the postfix formatted string list
        static double CalculateAnswer(List<string> postfixExp)
        {
            // create new temporary stack
            Stack<double> tempStack2 = new Stack<double>();

            //for each of the strings
            foreach (String str in postfixExp)
            {
                //if its a number either positive or negative
                char c = str[0];
                if (Char.IsDigit(c) ||(c == '-' && str.Length > 1))
                {
                    //convert it into a double and add to the stack
                    var n = Convert.ToDouble(str);
                    tempStack2.Push(n);
                }

                //if the scanned string is an operator that only takes one operand (factorial) calculate
                else if(str == "!")
                {
                    double num1 = tempStack2.Pop();
                    tempStack2.Push(CalculateFactorial(num1));
                }

                //if the scanned string is an operator, pop two elements from stack apply the operator then push back
                else
                {
                    double n1 = tempStack2.Pop();
                    double n2 = tempStack2.Pop();

                    switch (str)
                    {
                        case "+":
                            tempStack2.Push(n2 + n1);
                            break;

                        case "-":
                            tempStack2.Push(n2 - n1);
                            break;

                        case "/":
                            tempStack2.Push(n2 / n1);
                            break;

                        case "*":
                            tempStack2.Push(n2 * n1);
                            break;

                        case "^":
                            tempStack2.Push(Math.Pow(n2, n1));
                            break;

                    }
                }
            }
            return tempStack2.Pop();
        }

        //method to calculate the factorial of a number
        static double CalculateFactorial(double inputNum)
        {
            //added error handling in the factorial method to make sure that the program doesnt crash by tring to factorialise a negative number
            if (inputNum < 0)
            {
                Console.Error.WriteLine("cant factorialise a negative number");
                return inputNum;
            }
            else
            {
                //factorialise the number
                double factorialNum = inputNum;
                for (double i = factorialNum - 1; i > 0; i--)
                {
                    factorialNum *= i;
                }
                return factorialNum;
            }
        }
    }
}
