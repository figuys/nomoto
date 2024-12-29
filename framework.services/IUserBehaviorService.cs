using System.Collections.Generic;
using lenovo.mbg.service.framework.services.Model;

namespace lenovo.mbg.service.framework.services;

public interface IUserBehaviorService
{
	void InitUser(string user);

	void Collect(BusinessType business, BusinessData data);

	List<BehaviorModel> GetAll();
}
