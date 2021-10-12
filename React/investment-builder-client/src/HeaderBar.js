import 'bootstrap/dist/css/bootstrap.min.css';
import {Navbar, Nav, Form, FormControl, Button} from 'react-bootstrap';

const HeaderBar = () =>
{
    return(
        <Navbar bg="primary" variant="dark">
            <Navbar.Brand href="#home">Investment Builder</Navbar.Brand>
            <Nav className="mr-auto">
                <Nav.Link href="#home">Home</Nav.Link>
            </Nav >
            <Form inline>
                <FormControl type="text" placeholder="Search" className="mr-sm-2" />
                <Button variant="outline-light">Search</Button>
            </Form>            
        </Navbar>

    );
}

export default HeaderBar;