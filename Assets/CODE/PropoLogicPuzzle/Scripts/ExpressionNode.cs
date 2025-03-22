using System.Collections.Generic;

public abstract class ExpressionNode 
{
    public abstract string ToLogicString();
}

public class VariableNode : ExpressionNode 
{
    public string variable;
    public VariableNode(string var) { variable = var; }
    public override string ToLogicString() { return variable; }
}

public class ConstantNode : ExpressionNode 
{
    public bool value;
    public ConstantNode(bool val) { value = val; }
    public override string ToLogicString() { return value ? "true" : "false"; }
}

public enum OperatorType { Not, And, Or, Implies, Biconditional }

public class OperatorNode : ExpressionNode 
{
    public OperatorType op;
    public List<ExpressionNode> operands;
    
    public OperatorNode(OperatorType op, params ExpressionNode[] ops)
    {
        this.op = op;
        operands = new List<ExpressionNode>(ops);
    }
    
    public override string ToLogicString()
    {
        switch(op)
        {
            case OperatorType.Not:
                return "~" + operands[0].ToLogicString();
            case OperatorType.And:
                return "(" + operands[0].ToLogicString() + " ^ " + operands[1].ToLogicString() + ")";
            case OperatorType.Or:
                return "(" + operands[0].ToLogicString() + " v " + operands[1].ToLogicString() + ")";
            case OperatorType.Implies:
                return "(" + operands[0].ToLogicString() + " => " + operands[1].ToLogicString() + ")";
            case OperatorType.Biconditional:
                return "(" + operands[0].ToLogicString() + " <=> " + operands[1].ToLogicString() + ")";
            default:
                return "";
        }
    }
}
