using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator1
{

    /* Нужно добавить различные проверки при вводе(Коректность поступающего выражения)
     * проверка на два подряд стоящих знака 1+/3
     * проверка на точки 122.607.88
     * проверка на кол-во скобок (1+2)+7)   
     */

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Mather
        {
            public string infixExpr { get; private set; }
            public string postfixExpr { get; private set; }

            
            private Dictionary<char, int> operationPriority = new Dictionary<char, int>() {
        {'(', 0},
        {'+', 1},
        {'-', 1},
        {'*', 2},
        {'/', 2},
        {'%', 2},
        {'^', 3},
        {'√', 3},	
    };

            
            public Mather(string expression)
            {
                infixExpr = expression;
                postfixExpr = ToPostfix(infixExpr + "\r");
            }

            private string GetStringNumber(string expr, ref int pos)
            {
                string strNumber = "";

                for (; pos < expr.Length; pos++)
                {
                    char num = expr[pos];

                    if (Char.IsDigit(num))
                        strNumber += num;
                    else
                    {
                        pos--;
                        break;
                    }
                }

                return strNumber;
            }
            private string ToPostfix(string infixExpr)
            {

                string postfixExpr = "";

                Stack<char> stack = new Stack<char>();

                for (int i = 0; i < infixExpr.Length; i++)
                {
                    char c = infixExpr[i];

                    if (Char.IsDigit(c))
                    {
                        postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
                    }

                    else if (c == '(')
                    {
                        stack.Push(c);
                    }

                    else if (c == ')')
                    {
                        while (stack.Count > 0 && stack.Peek() != '(')
                            postfixExpr += stack.Pop();
                        
                        stack.Pop();
                    }
                    
                    else if (operationPriority.ContainsKey(c))
                    {
                       
                        char op = c;
                       
                        if (op == '-' && (i == 0 || (i > 1 && operationPriority.ContainsKey(infixExpr[i - 1]))))
                            op = '~';

                       
                        while (stack.Count > 0 && (operationPriority[stack.Peek()] >= operationPriority[op]))
                            postfixExpr += stack.Pop();
                        
                        stack.Push(op);
                    }
                }
               
                foreach (char op in stack)
                    postfixExpr += op;

               
                return postfixExpr;
            }

            private double Execute(char op, double first, double second)
            {
                switch (op)
                {
                    case '+':
                        return (first + second);
                        
                    case '-':
                        return first - second;
                    
                    case '*':
                        return first * second;

                    case '/':
                        return first / second;

                    case '^':
                        return Math.Pow(first, second);

                    case '√':
                        return Math.Pow(second, 0.5);

                    case '%':
                        return first * second / 100;

                    default:
                        return 0;
                        
                }
            }

            public double Calc()
            {
                
                Stack<double> locals = new Stack<double>();
               
                int counter = 0;

                
                for (int i = 0; i < postfixExpr.Length; i++)
                {
                    
                    char c = postfixExpr[i];

                   
                    if (Char.IsDigit(c))
                    {
                        string number = GetStringNumber(postfixExpr, ref i);
                        locals.Push(Convert.ToDouble(number));
                    }
                   
                    else if (operationPriority.ContainsKey(c))
                    {
                        counter += 1;

                        double second = locals.Count > 0 ? locals.Pop() : 0,
                        first = locals.Count > 0 ? locals.Pop() : 0;

                        locals.Push(Execute(c, first, second));
                       
                        
                    }
                }

                
                return locals.Pop();
            }
        }

        private void buttonSeven_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            textBoxQuestion.Text += b.Text; 
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            textBoxQuestion.Text += button.Text;
        }

        private void buttonAC_Click(object sender, EventArgs e)
        {
            textBoxQuestion.Clear();
            textBoxPolski.Clear();
            textBoxAnswer.Clear();
        }

        private void buttonDot_Click(object sender, EventArgs e)
        {
            textBoxQuestion.Text += buttonDot.Text;
        }

        private void buttonEqual_Click(object sender, EventArgs e)
        {
            Mather mather = new Mather(textBoxQuestion.Text);
            textBoxPolski.Text = mather.postfixExpr;
            textBoxAnswer.Text = Convert.ToString(mather.Calc());
            
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Принимает только корректные выражения, отсутствуют проверки");
        }
    }
}
