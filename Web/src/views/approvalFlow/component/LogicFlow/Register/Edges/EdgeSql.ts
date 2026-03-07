import { BezierEdge, BezierEdgeModel } from '@logicflow/core';

class CustomEdge2 extends BezierEdge { }

class CustomEdgeModel2 extends BezierEdgeModel {
    getEdgeStyle() {
        const style = super.getEdgeStyle();
        // svg attribute
        style.strokeWidth = 1;
        style.stroke = '#ababac';
        return style;
    }
    /**
     * Override this method，makesaveDataYesCan bring an anchor pointData。
     */
    getData() {
        const data = super.getData();
        data.sourceAnchorId = this.sourceAnchorId;
        data.targetAnchorId = this.targetAnchorId;
        return data;
    }
    /**
     * Give the edgeCustomizePlan，Make it support anchor-based positioningUpdatePath of the edge
     */
    updatePathByAnchor() {
        // TODO
        const sourceNodeModel = this.graphModel.getNodeModelById(this.sourceNodeId);
        const sourceAnchor = sourceNodeModel
            .getDefaultAnchor()
            .find((anchor) => anchor.id === this.sourceAnchorId);
        const targetNodeModel = this.graphModel.getNodeModelById(this.targetNodeId);
        const targetAnchor = targetNodeModel
            .getDefaultAnchor()
            .find((anchor) => anchor.id === this.targetAnchorId);
        const startPoint = {
            x: sourceAnchor.x,
            y: sourceAnchor.y,
        };
        this.updateStartPoint(startPoint);
        const endPoint = {
            x: targetAnchor.x,
            y: targetAnchor.y,
        };
        this.updateEndPoint(endPoint);
        // Here you need to set the original pointsList to empty to trigger bezier's automatic calculation of control points.
        this.pointsList = [];
        this.initPoints();
    }
}

export default {
    type: 'edge-sql',
    view: CustomEdge2,
    model: CustomEdgeModel2,
};