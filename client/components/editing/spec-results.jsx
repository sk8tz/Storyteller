var React = require('react');
var Postal = require('postal');
var {connect} = require('react-redux');
var {Button, ButtonGroup, Grid, Row, Col, ListGroup, ListGroupItem} = require('react-bootstrap');
var EditorLoading = require('./alerts/editor-loading');
var SpecHeader = require('./header/spec-header');
var SpecResultHeader = require('./header/spec-result-header');
var loader = require('./component-loader').results;

function getSpec(state, ownProps){
    var id = ownProps.params.id;
    var spec = state.get('specs').get(id);
    var loading = spec.mode == 'header';

    if (loading){
        // TODO -- do this differently
        Postal.publish({
            channel: 'engine-request',
            topic: 'spec-data-requested',
            data: {
                type: 'spec-data-requested',
                id: id
            }
        });
    }
    
    return {spec: spec, loading: loading};
}

function addDispatch(dispatch){
    return {dispatch: dispatch};
}

function SpecResults(props){
    if (props.loading){
        return ( <EditorLoading spec={props.spec} /> );
    }
    
    // <Persisting spec={this.props.spec} lastSaved={this.state.lastSaved} persisting={this.state.persisting}/>
    
    var components = props.spec.buildResults(loader);
    
    return (
        <Grid>
            <SpecHeader spec={props.spec} mode='results' />
            <Row>
                <SpecResultHeader spec={props.spec} />
                {components}
            </Row>
        </Grid>
    );
}



module.exports = connect(getSpec, addDispatch)(SpecResults);