/** @jsx React.DOM */

var React = require("react");
var {Row, Col} = require('react-bootstrap');

var SpecTitle = require('./spec-title');
var SpecLinks = require('./spec-links');
var SpecCommands = require('./spec-commands');
var LifecycleButton = require('./lifecycle-button');

var SpecHeader = React.createClass({
	render(){
		// Hokey, but letting it pass
		var headerClass = "";
		if (this.props.mode == 'editor' && this.props.spec.active){
			headerClass = "text-primary";
		}

		return (
			<Row>
				<Col xs={12} md={12}>
				    <h3 ref="header" className={headerClass}>
						<SpecTitle spec={this.props.spec} />
						<span className="pull-right">
							<SpecCommands spec={this.props.spec}/>

							<SpecLinks id={this.props.spec.id} mode={this.props.mode} />

							<LifecycleButton spec={this.props.spec} />
						</span>
					</h3>

					<hr />
				</Col>
			</Row>
		);
	}
});

module.exports = SpecHeader;