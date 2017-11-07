using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Services.Core.Services
{
	public interface IService
	{
		void Start();
		void Stop();
		void Restart();
	}
}
