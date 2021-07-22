import 'bootstrap/dist/css/bootstrap.min.css';
import {Container, Row, Col } from 'react-bootstrap';
import Testcard from './TestCard.js';
import TestNav from './TestNav.js'
import TestTabs from './TestTabs.js'

const Testbootstrap = () =>
{
    return(
        <Container fluid>
            <Row>
                <Col>
                    <TestNav/>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Testcard/>
                </Col>
            </Row> 
            <Row>
                <Col>
                    <TestTabs/>
                </Col>
            </Row>
        </Container> 
    );
}

export default Testbootstrap;