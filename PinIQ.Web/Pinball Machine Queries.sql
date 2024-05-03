select [kw].[Name], [m].[Name]
from [PinballMachineKeywordMapping] km
    join [PinballKeywords] kw
on km.KeywordId = kw.Id
    join [PinballMachines] m on km.MachineId = m.Id

select pm.[Name], pm.Id, t.[Name] 'Type'
from [PinballMachines] pm
    left join [PinballTypes] t
on pm.TypeId = t.Id