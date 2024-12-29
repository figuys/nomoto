using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class CountrySources
{
	private static readonly List<string> mCountrys;

	static CountrySources()
	{
		mCountrys = new List<string>
		{
			"Algeria", "Angola", "Argentina", "Belgium", "Australia", "Austria", "Bahrain", "Bangladesh", "Belarus", "Benin",
			"Bolivia", "Botswana", "Brazil", "Bulgaria", "Burkina", "Burundi", "Cameroon", "Canada", "Cape Verde", "Central African Republic",
			"Central America", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo, Dem. Rep. (Kinshasa)", "Congo, Rep. (Brazzaville)", "Cote d'Ivoire", "Croatia",
			"Czech Republic", "Denmark", "Deutschland", "Djibouti", "Ecuador", "Egypt", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia",
			"Finland", "France", "Gabon", "The Gambia", "Germany", "Ghana", "Greece", "Guinea", "Guinea-Bissau", "Hong Kong S.A.R. of China",
			"Hungary", "India", "Indonesia", "Ireland", "Israel", "Italy", "Japan", "Jordan", "Kazakhstan", "Kenya",
			"Korea", "Kuwait", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Lithuania", "Macedonia", "Madagascar",
			"Malawi", "Malaysia", "Mali", "Mauritania", "Mauritius", "Mexico", "Moldova", "Morocco", "Mozambique", "Myanmar",
			"Namibia", "Netherlands", "New Zealand", "Niger", "Nigeria", "Norway", "Oman", "Pakistan", "Paraguay", "Peru",
			"Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russian Federation", "Rwanda", "Réunion", "Saint Helena", "Saudi Arabia",
			"Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Somalia", "South Africa", "South Sudan",
			"Spain", "Sri Lanka", "Sudan", "Swaziland", "Sweden", "Switzerland", "São Tomé and Príncipe", "Taiwan Region", "Tanzania", "Thailand",
			"Togo", "Tunisia", "Turkey", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States", "Uruguay", "Venezuela",
			"Vietnam", "Western Sahara", "Zambia", "Zimbabwe"
		};
		mCountrys.Sort();
	}

	public List<string> GetCountrys()
	{
		return mCountrys;
	}
}
