import { CircleNode, CircleNodeModel, h } from "@logicflow/core";

class StartNode extends CircleNode {
    getLabelShape() {
        const { model } = this.props;
        const { x, y } = model;
        return h('text', { fill: '#000000', fontSize: 12, x: x - 12, y: y + 4, width: 50, height: 25 }, 'Start');
    }
    getShape() {
        const { model } = this.props;
        const { x, y, r } = model;
        const { fill, stroke, strokeWidth } = model.getNodeStyle();
        return h('g', {}, [h('circle', { cx: x, cy: y, r, fill, stroke, strokeWidth }), this.getLabelShape()]);
    }
}

class StartModel extends CircleNodeModel {
    // Custom node shape properties
    initNodeData(data) {
        data.text = {
            value: (data.text && data.text.value) || '',
            x: data.x,
            y: data.y + 35,
            dragable: false,
            editable: true
        }
        super.initNodeData(data)
        this.r = 20
    }
    // Custom node style properties
    getNodeStyle() {
        const style = super.getNodeStyle()
        return style
    }
    // Custom anchor style
    getAnchorStyle() {
        const style = super.getAnchorStyle()
        style.hover.r = 8
        style.hover.fill = 'rgb(24, 125, 255)'
        style.hover.stroke = 'rgb(24, 125, 255)'
        return style
    }
    // Custom node outline
    getOutlineStyle() {
        const style = super.getOutlineStyle()
        style.stroke = '#88f'
        return style
    }
    getConnectedTargetRules() {
        const rules = super.getConnectedTargetRules()
        const notAsTarget = {
            message: 'The starting node cannot be used as the endpoint of a connection',
            validate: () => false
        }
        rules.push(notAsTarget)
        return rules
    }
}

export default {
    type: 'start-node',
    view: StartNode,
    model: StartModel
}