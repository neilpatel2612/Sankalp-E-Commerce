CREATE TABLE [dbo].[SKL_Cader](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cader] [nvarchar](50) NULL,
	[ColorCodeId] [int] NULL,
 CONSTRAINT [PK_SKL_Cader] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[SKL_ColorCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColorCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_SKL_ColorCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO c (LanguageId,ResourceName,ResourceValue)
SELECT 1 , 'Admin.Sankalp' , 'Sankalp Custom'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader' , 'Cader'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode' , 'Color Code'

UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Manage' , 'Manage Cader'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.List.SearchCaderName' , 'Search Cader Name'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Fields.Name' , 'Cader'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.EditCaderDetails' , 'Edit Cader'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.AddNew' , 'Add Cader'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Info' , 'Info'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.BackToList' , 'Back To List'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Added' , 'The new attribute has been added successfully.'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Updated' , 'The attribute has been updated successfully.'
UNION ALL SELECT 1 , 'Admin.Sankalp.Cader.Deleted' , 'The attribute has been deleted successfully.'

UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Manage' , 'Manage Color Code'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.List.SearchColorCodeName' , 'Search Color Code Name'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Fields.Name' , 'Color Code'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.EditCaderDetails' , 'Edit Color Code'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.AddNew' , 'Add Color Code'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Info' , 'Info'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.BackToList' , 'Back To List'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Added' , 'The new attribute has been added successfully.'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Updated' , 'The attribute has been updated successfully.'
UNION ALL SELECT 1 , 'Admin.Sankalp.ColorCode.Deleted' , 'The attribute has been deleted successfully.'