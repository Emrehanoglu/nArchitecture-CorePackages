using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Transaction;

//sadece imza niyetine kullanacağım için burada boş bir interface olusturdum 
//bu imza benim işime TransactionScopeBehavior 'a gelecek TRequest 'i imzalarken yarayacak.
//bir de hangi command 'de kullanıyorsam örneğin CreateBrandCommand classına imza olarak ekledim.
//çünkü burada CreateBrandCommand benim TransactionScopeBehavior 'daki TRequest 'ime karşılık gelmektedir.
public interface ITransactionalRequest 
{
}
