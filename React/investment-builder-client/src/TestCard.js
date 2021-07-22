import 'bootstrap/dist/css/bootstrap.min.css';
import {Card} from 'react-bootstrap';

const Testcard = () =>
{
    return(
        <Card bg="light" className="mt-sm-3">
            <Card.Body>This can contain all the summary information</Card.Body>
        </Card>
    );
}

export default Testcard;