import LogicFlow from "@logicflow/core";
// Introduce custom edges
import edgeSql from './Edges/EdgeSql';
// Register side
const Register = (lf: LogicFlow) => {
    lf.register(edgeSql);
};

export default { Register };