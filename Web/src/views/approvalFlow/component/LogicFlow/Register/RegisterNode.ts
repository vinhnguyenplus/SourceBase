import LogicFlow from "@logicflow/core";
// Introduce custom nodes
import nodeStart from './Nodes/NodeStart';
import nodeEnd from './Nodes/NodeEnd';
import nodeTask from './Nodes/NodeTask';
import nodeUser from './Nodes/NodeUser';
import nodeSql from './Nodes/NodeSql';
// Register node
const Register = (lf: LogicFlow) => {
    lf.register(nodeStart);
    lf.register(nodeEnd);
    lf.register(nodeTask);
    lf.register(nodeUser);
    lf.register(nodeSql);
};

export default { Register };