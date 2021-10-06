import 'bootstrap/dist/css/bootstrap.min.css';
import {Card} from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js";

const Testcard = () =>
{
    const [accountName, setAccountName] = useState('');
    const [reportingCurrency, setReportingCurrency] = useState('');
    const [valuePerUnit, setValuePerUnit] = useState('');
    const [netAssets, setNetAssets] = useState('');
    const [bankBalance, setBankBalance] = useState('');
    const [monthlyPnL, setmonthlyPnL] = useState('');
    const [valuationDate, setValuationDate] = useState('');

    const onLoadAccountSummary = function (response) {
        setAccountName( response.AccountName );
        setReportingCurrency( response.ReportingCurrency );
        setValuePerUnit( response.ValuePerUnit );
        setNetAssets( response.NetAssets );
        setBankBalance( response.BankBalance );
        setmonthlyPnL( response.MonthlyPnL );

        var dtValuation = new Date(response.ValuationDate);
        setValuationDate( dtValuation.toDateString() );
    };

    const loadAccountSummary = function () {
        middlewareService.GetInvestmentSummary(onLoadAccountSummary);
    }

    notifyService.RegisterConnectionListener(loadAccountSummary);

    notifyService.RegisterAccountListener(loadAccountSummary);

    return(
        <Card bg="light" className="mt-sm-3">
            <Card.Header as="h3">{accountName}</Card.Header>
            <Card.Body>
                <Card.Title></Card.Title>
            </Card.Body>
        </Card>
    );
}

export default Testcard;