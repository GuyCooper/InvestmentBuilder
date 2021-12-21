import { ListGroup, Modal } from 'react-bootstrap';

const ReportCompletion = function(props) {

    return(
        <Modal
            show={props.show}
            aria-labelledby="contained-modal-title-vcenter"
            centered
            onHide={() => props.onHide()}>
            <Modal.Header closeButton>
                {props.success === true && <Modal.Title
                                                id="contained-modal-title-vcenter">
                                                    Build Report Completed
                                            </Modal.Title>}
                {props.success === false && <Modal.Title
                                                id="contained-modal-title-vcenter">
                                                    Build Report Failed
                                            </Modal.Title>}                                            
            </Modal.Header>            
            <Modal.Body>
                {
                    props.success === false &&
                    <ListGroup>
                        {
                            props.errors.map( (e, i) => (
                                  <ListGroup.Item key={i}>                                        
                                        {e}
                                  </ListGroup.Item>    
                            ))
                        }
                    </ListGroup>    
                }
            </Modal.Body>
            <Modal.Footer>
                {props.success === true && <a href={props.completedReport} target="_blank" rel="noreferrer">Show Report</a>}
            </Modal.Footer>
        </Modal>
    );
};

export default ReportCompletion;