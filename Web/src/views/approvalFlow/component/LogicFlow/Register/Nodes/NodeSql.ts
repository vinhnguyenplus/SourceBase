import { HtmlNode, HtmlNodeModel, h } from '@logicflow/core';

class SqlNode extends HtmlNode {
    /**
     * 1.1.7Supported after the versionviewinRewrite anchor shape。
     * Rewrite AnchorAdd New
     */
    getAnchorShape(anchorData) {
        const { x, y, type } = anchorData;
        return h('rect', {
            x: x - 5,
            y: y - 5,
            width: 10,
            height: 10,
            className: `custom-anchor ${type === 'left' ? 'incomming-anchor' : 'outgoing-anchor'
                }`,
        });
    }
    setHtml(rootEl) {
        rootEl.innerHTML = '';
        const {
            properties: { fields, tableName },
        } = this.props.model;
        rootEl.setAttribute('class', 'table-container');
        const container = document.createElement('div');
        container.className = `table-node table-color-${Math.ceil(
            Math.random() * 4,
        )}`;
        const tableNameElement = document.createElement('div');
        tableNameElement.innerText = tableName;
        tableNameElement.className = 'table-name';
        container.appendChild(tableNameElement);
        const fragment = document.createDocumentFragment();
        for (let i = 0; i < fields.length; i++) {
            const item = fields[i];
            const itemElement = document.createElement('div');
            itemElement.className = 'table-feild';
            const itemKey = document.createElement('span');
            itemKey.innerText = item.key;
            const itemType = document.createElement('span');
            itemType.innerText = item.type;
            itemType.className = 'feild-type';
            itemElement.appendChild(itemKey);
            itemElement.appendChild(itemType);
            fragment.appendChild(itemElement);
        }
        container.appendChild(fragment);
        rootEl.appendChild(container);
    }
}

class SqlNodeModel extends HtmlNodeModel {
    /**
     * givemodelCustomizeAdd toFieldMethod
     */
    addField(item) {
        this.properties.fields.unshift(item);
        this.setAttributes();
        // In order to keep the position of the top of the node unchanged, after the node changes, the node is displaced, and the displacement distance is half of the added height.
        this.move(0, 24 / 2);
        // Update the path of the node connecting the edge
        this.incoming.edges.forEach((egde) => {
            // Call a custom update plan
            egde.updatePathByAnchor();
        });
        this.outgoing.edges.forEach((edge) => {
            // Call a custom update plan
            edge.updatePathByAnchor();
        });
    }
    getOutlineStyle() {
        const style = super.getOutlineStyle();
        style.stroke = 'none';
        style.hover.stroke = 'none';
        return style;
    }
    // If you do not need to modify the anchorage shape, you can override the color-related styles
    getAnchorStyle(anchorInfo) {
        const style = super.getAnchorStyle();
        if (anchorInfo.type === 'left') {
            style.fill = 'red';
            style.hover.fill = 'transparent';
            style.hover.stroke = 'transpanrent';
            style.className = 'lf-hide-default';
        } else {
            style.fill = 'green';
        }
        return style;
    }
    setAttributes() {
        this.width = 200;
        const {
            properties: { fields },
        } = this;
        this.height = 60 + fields.length * 24;
        const circleOnlyAsTarget = {
            message: 'Only allow connections from the right anchor point',
            validate: (sourceNode, targetNode, sourceAnchor) => {
                return sourceAnchor.type === 'right';
            },
        };
        this.sourceRules.push(circleOnlyAsTarget);
        this.targetRules.push({
            message: 'Only allow connections to the left anchor',
            validate: (sourceNode, targetNode, sourceAnchor, targetAnchor) => {
                return targetAnchor.type === 'left';
            },
        });
    }
    getDefaultAnchor() {
        const {
            id,
            x,
            y,
            width,
            height,
            isHovered,
            isSelected,
            properties: { fields, isConnection },
        } = this;
        const anchors = [];
        fields.forEach((feild, index) => {
            // If it is connected, the anchor point on the left will not be displayed.
            if (isConnection || !(isHovered || isSelected)) {
                anchors.push({
                    x: x - width / 2 + 10,
                    y: y - height / 2 + 60 + index * 24,
                    id: `${id}_${feild.key}_left`,
                    edgeAddable: false,
                    type: 'left',
                });
            }
            if (!isConnection) {
                anchors.push({
                    x: x + width / 2 - 10,
                    y: y - height / 2 + 60 + index * 24,
                    id: `${id}_${feild.key}_right`,
                    type: 'right',
                });
            }
        });
        return anchors;
    }
}

export default {
    type: 'sql-node',
    model: SqlNodeModel,
    view: SqlNode,
};